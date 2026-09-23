using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IIDController : MonoBehaviour
{
    [SerializeField] GameObject FlashLight;
    private bool FlashLightIsOn;
    public InputAction FlashLightToggle;

    //added this so we can check if the player actually owns the IID
    //eventually the shop will change this when the player buys one
    [SerializeField] private bool ownsIID = true;

    [Header("Battery Settings")]

    //how much charge a fresh battery has
    [SerializeField] private float maxBatteryCharge = 100f;

    //how much charge is currently left in the battery inside the IID
    [SerializeField] private float batteryCharge = 100f;

    //how much battery the flashlight uses every second while turned on
    [SerializeField] private float batteryDrainPerSecond = 5f;

    //how many extra batteries the player is currently carrying
    //eventually we'll get these from vending machines
    [SerializeField] private int spareBatteries = 3;

    [Header("Reload Settings")]

    //keeping reload as its own InputAction so we can change the button later if we want
    public InputAction ReloadIID;

    [Header("Light Damage Settings")]

    //how much damage the IID deals every second an enemy is kept in the beam
    [SerializeField] private float lightDamagePerSecond = 25f;

    //only objects on these layers will be checked as possible enemies
    [SerializeField] private LayerMask enemyLayer;

    //these layers should block the flashlight from hitting an enemy
    //stuff like walls, buildings, trees, etc.
    [SerializeField] private LayerMask obstacleLayer;

    [Header("IID Overload Settings")]

    //keeping overload as its own InputAction so we can change the button later
    public InputAction OverloadIID;

    //anything this close gets completely fried by the explosion
    [SerializeField] private float overloadKillRadius = 4f;

    //anything inside this radius but outside the kill radius
    //takes damage and gets stunned
    [SerializeField] private float overloadBlastRadius = 8f;

    //how much damage enemies outside the instant kill radius take
    [SerializeField] private float overloadDamage = 50f;

    //how long surviving enemies are stunned after the explosion
    [SerializeField] private float overloadStunDuration = 5f;

    //grab the actual Unity light so our gameplay cone can match the visible flashlight
    private Light spotLight;

    //keeps track of which enemies were in the beam during the previous frame
    //this lets us tell when the flashlight has moved OFF of an enemy
    private HashSet<EnemyLightReaction> previouslyLitEnemies =
        new HashSet<EnemyLightReaction>();

    void Start()
    {
        FlashLightToggle.Enable();

        //enable the reload input too
        ReloadIID.Enable();

        //enable the overload input
        OverloadIID.Enable();

        //grab the actual Spot Light from the flashlight object
        spotLight = FlashLight.GetComponentInChildren<Light>();

        if (spotLight == null)
        {
            Debug.LogWarning("IID could not find a Light component!");
        }

        //making sure the flashlight always starts turned off
        FlashLight.SetActive(false);
        FlashLightIsOn = false;
    }

    void Update()
    {
        if (FlashLightToggle.WasPressedThisFrame())
        {
            TurnOnFlashLight();
        }

        //if the flashlight is currently on, drain the loaded battery
        //and check which enemies are currently inside the beam
        if (FlashLightIsOn)
        {
            DrainBattery();

            //DrainBattery might have shut the flashlight off this frame
            //so only check for enemies if we STILL have power
            if (FlashLightIsOn)
            {
                CheckForEnemiesInLight();
            }
        }

        //reload is completely manual
        //running out of charge will NOT automatically use another battery
        if (ReloadIID.WasPressedThisFrame())
        {
            ReloadBattery();
        }

        //our emergency "OH CRAP" button
        //this can still be used even if the loaded battery is completely dead
        if (OverloadIID.WasPressedThisFrame())
        {
            Overload();
        }
    }

    public void TurnOnFlashLight()
    {
        //don't let the player turn it on if they haven't bought the IID
        if (!ownsIID)
        {
            Debug.Log("Player does not own an IID");
            return;
        }

        //don't let the player turn the flashlight on with a dead battery
        if (!FlashLightIsOn && batteryCharge <= 0f)
        {
            Debug.Log("IID battery is dead - Reload!");
            return;
        }

        if (!FlashLightIsOn)
        {
            FlashLight.SetActive(true);
            FlashLightIsOn = true;
            Debug.Log("Flash Light Turned on");
        }
        else if (FlashLightIsOn)
        {
            TurnOffFlashLight();
        }
    }

    //moved shutting the flashlight off into its own method
    //this makes sure any enemies we're stunning get released when the light shuts off
    private void TurnOffFlashLight()
    {
        FlashLightIsOn = false;
        FlashLight.SetActive(false);

        ReleaseLitEnemies();
    }

    private void DrainBattery()
    {
        //drain battery based on how long the flashlight has actually been running
        batteryCharge -= batteryDrainPerSecond * Time.deltaTime;

        //don't allow the battery charge to go below 0
        if (batteryCharge <= 0f)
        {
            batteryCharge = 0f;

            //the battery died, so shut the light off
            //IMPORTANT: we are NOT automatically reloading here
            TurnOffFlashLight();

            Debug.Log("IID battery died!");
        }
    }

    private void ReloadBattery()
    {
        //can't reload something we don't own
        if (!ownsIID)
        {
            Debug.Log("Player does not own an IID");
            return;
        }

        //don't use a spare battery if the current battery is already full
        if (batteryCharge >= maxBatteryCharge)
        {
            Debug.Log("IID battery is already full");
            return;
        }

        //can't reload if we don't have another battery
        if (spareBatteries <= 0)
        {
            Debug.Log("No spare batteries!");
            return;
        }

        //use one spare battery
        spareBatteries--;

        //replace the current battery with a completely fresh one
        //any charge left in the old battery is lost
        batteryCharge = maxBatteryCharge;

        Debug.Log("IID reloaded! Spare batteries remaining: " + spareBatteries);
    }

    private void CheckForEnemiesInLight()
    {
        if (spotLight == null)
            return;

        //make a fresh list of everybody being lit THIS frame
        HashSet<EnemyLightReaction> currentlyLitEnemies =
            new HashSet<EnemyLightReaction>();

        //look for enemy colliders anywhere within the flashlight's maximum range
        Collider[] nearbyEnemies = Physics.OverlapSphere(
            spotLight.transform.position,
            spotLight.range,
            enemyLayer
        );

        foreach (Collider enemyCollider in nearbyEnemies)
        {
            //the collider might be on a child of the enemy
            //so search upward until we find EnemyLightReaction
            EnemyLightReaction enemy =
                enemyCollider.GetComponentInParent<EnemyLightReaction>();

            if (enemy == null)
                continue;

            //aim roughly toward the center of the enemy's collider
            Vector3 targetPosition = enemyCollider.bounds.center;

            Vector3 directionToEnemy =
                targetPosition - spotLight.transform.position;

            float distanceToEnemy = directionToEnemy.magnitude;

            //check whether the enemy is actually inside the flashlight cone
            float angleToEnemy = Vector3.Angle(
                spotLight.transform.forward,
                directionToEnemy.normalized
            );

            //spotAngle is the FULL cone, so we only want half of it on either side
            if (angleToEnemy > spotLight.spotAngle * 0.5f)
                continue;

            //make sure a wall/tree/building isn't between the flashlight and enemy
            if (Physics.Raycast(
                spotLight.transform.position,
                directionToEnemy.normalized,
                out RaycastHit hit,
                distanceToEnemy,
                obstacleLayer))
            {
                continue;
            }

            //if we got this far, the enemy is actually being illuminated
            currentlyLitEnemies.Add(enemy);

            enemy.EnterLight();

            //damage is based on time so framerate doesn't change how quickly enemies die
            enemy.TakeLightDamage(lightDamagePerSecond * Time.deltaTime);
        }

        //anyone who WAS lit but isn't anymore should recover from the stun
        foreach (EnemyLightReaction enemy in previouslyLitEnemies)
        {
            if (enemy != null && !currentlyLitEnemies.Contains(enemy))
            {
                enemy.ExitLight();
            }
        }

        previouslyLitEnemies = currentlyLitEnemies;
    }

    private void ReleaseLitEnemies()
    {
        //if the player manually turns the IID off, or the battery dies,
        //every enemy currently being stunned needs to be released
        foreach (EnemyLightReaction enemy in previouslyLitEnemies)
        {
            if (enemy != null)
            {
                enemy.ExitLight();
            }
        }

        previouslyLitEnemies.Clear();
    }

    private void Overload()
    {
        //can't blow up an IID we don't actually own
        if (!ownsIID)
        {
            Debug.Log("Player does not own an IID");
            return;
        }

        Debug.Log("IID OVERLOAD!");

        //shut the normal flashlight off first
        //this also releases anything currently being held by the normal beam
        if (FlashLightIsOn)
        {
            TurnOffFlashLight();
        }

        //find every enemy collider inside the full blast radius
        Collider[] enemiesInBlast = Physics.OverlapSphere(
            transform.position,
            overloadBlastRadius,
            enemyLayer
        );

        //some enemies may have multiple colliders
        //this keeps us from accidentally hitting the same monster multiple times
        HashSet<EnemyLightReaction> hitEnemies =
            new HashSet<EnemyLightReaction>();

        foreach (Collider enemyCollider in enemiesInBlast)
        {
            EnemyLightReaction enemy =
                enemyCollider.GetComponentInParent<EnemyLightReaction>();

            if (enemy == null || hitEnemies.Contains(enemy))
                continue;

            hitEnemies.Add(enemy);

            //measure how close this enemy is to the player
            float distanceToEnemy = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distanceToEnemy <= overloadKillRadius)
            {
                //anything this close gets completely fried
                enemy.KillInstantly();
            }
            else
            {
                //anything farther out takes heavy damage
                enemy.TakeLightDamage(overloadDamage);

                //if the damage didn't kill them, keep them stunned
                //long enough for the player to hopefully get the hell out of there
                if (enemy != null)
                {
                    enemy.StunForSeconds(overloadStunDuration);
                }
            }
        }

        //using the overload destroys the IID itself
        //IMPORTANT: spare batteries are intentionally left completely alone
        ownsIID = false;

        Debug.Log("IID destroyed!");
    }

    //draw the two explosion ranges in the Scene view
    //this is just for us while balancing and does NOT show up during gameplay
    private void OnDrawGizmosSelected()
    {
        //inner circle = instant death
        Gizmos.DrawWireSphere(transform.position, overloadKillRadius);

        //outer circle = damage + stun
        Gizmos.DrawWireSphere(transform.position, overloadBlastRadius);
    }
}