using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerGrab : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private float escapeRequired = 100f;
    [SerializeField] private float baseEscapePower = 10f;
    [SerializeField] private float grabImmunityTime = 2f;

    private float currentEscapeProgress;
    private float immunityTimer;

    private bool isGrabbed;

    private NavMeshAgent playerAgent;
    private PlayerStamina playerStamina;

    //the enemy currently grabbing the player
    private GameObject grabbingEnemy;

    //the escape bar above the grabbing enemy
    private Slider escapeBar;

    public bool IsGrabbed => isGrabbed;
    public bool CanBeGrabbed => !isGrabbed && immunityTimer <= 0f;

    private void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        playerStamina = GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        //count down temporary immunity after escaping
        if (immunityTimer > 0f)
        {
            immunityTimer -= Time.deltaTime;
        }

        if (!isGrabbed)
            return;

        //press E repeatedly to escape
        if (Input.GetKeyDown(KeyCode.E))
        {
            AttemptEscape();
        }
    }

    public void BeginGrab(GameObject enemy)
    {
        //don't allow another grab while already grabbed
        //or during post-grab immunity
        if (!CanBeGrabbed)
            return;

        isGrabbed = true;
        currentEscapeProgress = 0f;

        grabbingEnemy = enemy;

        //find the escape bar on the grabbing enemy
        if (grabbingEnemy != null)
        {
            escapeBar = grabbingEnemy.GetComponentInChildren<Slider>(true);

            if (escapeBar != null)
            {
                escapeBar.gameObject.SetActive(true);
                escapeBar.minValue = 0f;
                escapeBar.maxValue = escapeRequired;
                escapeBar.value = 0f;
            }
        }

        //stop the player immediately
        if (playerAgent != null)
        {
            playerAgent.ResetPath();
            playerAgent.isStopped = true;
        }

        Debug.Log("Player grabbed!");
    }

    private void AttemptEscape()
    {
        float escapePower = baseEscapePower;

        //stamina affects how effective each E press is
        if (playerStamina != null)
        {
            float staminaPercent =
                playerStamina.CurrentStamina /
                playerStamina.MaxStamina;

            float staminaMultiplier =
                Mathf.Lerp(
                    0.5f,
                    1.5f,
                    staminaPercent
                );

            escapePower *= staminaMultiplier;
        }

        currentEscapeProgress += escapePower;

        //update the bar above the enemy
        if (escapeBar != null)
        {
            escapeBar.value = currentEscapeProgress;
        }

        Debug.Log(
            "Escape Progress: "
            + currentEscapeProgress
            + " / "
            + escapeRequired
        );

        if (currentEscapeProgress >= escapeRequired)
        {
            EscapeGrab();
        }
    }

    private void EscapeGrab()
    {
        isGrabbed = false;
        currentEscapeProgress = 0f;

        immunityTimer = grabImmunityTime;

        //hide the escape bar again
        if (escapeBar != null)
        {
            escapeBar.gameObject.SetActive(false);
        }

        escapeBar = null;
        grabbingEnemy = null;

        //allow the player to move again
        if (playerAgent != null)
        {
            playerAgent.isStopped = false;
        }

        Debug.Log("Player escaped!");
    }
}