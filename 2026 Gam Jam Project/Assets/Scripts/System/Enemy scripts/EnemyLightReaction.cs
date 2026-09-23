using UnityEngine;

public class EnemyLightReaction : MonoBehaviour
{
    [Header("Light Damage Settings")]
    [SerializeField] private float maxLightHealth = 100f;

    //how much health the enemy currently has against light
    private float currentLightHealth;

    private EnemyAI enemyAI;

    //keeps track of whether the enemy is currently being hit by the IID
    private bool isInLight = false;

    //keeps track of stuns that should last for a specific amount of time
    //this will mostly be used for the IID overload
    private float timedStunRemaining = 0f;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();

        //start every enemy at full light health
        currentLightHealth = maxLightHealth;
    }

    private void Update()
    {
        //count down any timed stun currently affecting the enemy
        if (timedStunRemaining > 0f)
        {
            timedStunRemaining -= Time.deltaTime;

            if (timedStunRemaining <= 0f)
            {
                timedStunRemaining = 0f;

                //if the normal flashlight is STILL hitting the enemy,
                //don't let the timed stun ending make them start moving again
                if (!isInLight && enemyAI != null)
                {
                    enemyAI.SetStunned(false);
                }

                Debug.Log("Enemy recovered from timed stun!");
            }
        }
    }

    //called by the IID when this enemy is inside the flashlight beam
    public void EnterLight()
    {
        //don't keep telling the AI it's stunned every single frame
        if (!isInLight)
        {
            isInLight = true;

            if (enemyAI != null)
            {
                enemyAI.SetStunned(true);
            }

            Debug.Log("Enemy entered IID light!");
        }
    }

    //called by the IID when this enemy is no longer inside the flashlight beam
    public void ExitLight()
    {
        if (isInLight)
        {
            isInLight = false;

            //only let the enemy recover if there isn't ALSO
            //a timed stun currently keeping them stunned
            if (enemyAI != null && timedStunRemaining <= 0f)
            {
                enemyAI.SetStunned(false);
            }

            Debug.Log("Enemy left IID light!");
        }
    }

    //used for things like the IID overload where the enemy should stay
    //stunned for a certain amount of time after getting hit
    public void StunForSeconds(float duration)
    {
        //if the enemy is already stunned for longer than this,
        //don't accidentally replace it with a shorter stun
        timedStunRemaining = Mathf.Max(timedStunRemaining, duration);

        if (enemyAI != null)
        {
            enemyAI.SetStunned(true);
        }

        Debug.Log("Enemy stunned for " + duration + " seconds!");
    }

    //called while the enemy is being exposed to the flashlight
    public void TakeLightDamage(float damage)
    {
        currentLightHealth -= damage;

        //don't let health go below 0
        currentLightHealth = Mathf.Max(currentLightHealth, 0f);

        if (currentLightHealth <= 0f)
        {
            Die();
        }
    }

    public void KillInstantly()
    {
        Die();
    }

    private void Die()
    {
        Debug.Log("Enemy burned away!");

        //temporary death behavior
        //we can replace this with the actual burn-away effect later
        Destroy(gameObject);
    }
}