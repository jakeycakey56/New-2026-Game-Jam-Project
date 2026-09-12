using UnityEngine;
using UnityEngine.AI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 20f;
    [SerializeField] private float standingRegenMultiplier = 2.5f;
    [SerializeField] private float regenDelay = 1f;

    private float currentStamina;
    private float regenTimer;

    private NavMeshAgent playerAgent;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    public bool HasStamina => currentStamina > 0f;

    //used by hiding spots to force the player into the faster resting regen state
    public bool IsHiding { get; set; }

    private void Start()
    {
        currentStamina = maxStamina;

        //grab the player's NavMeshAgent so we can check if they're moving
        playerAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (currentStamina < maxStamina)
        {
            //check whether the player is currently moving
            bool isMoving =
                playerAgent != null &&
                playerAgent.velocity.sqrMagnitude > 0.01f;

            float currentRegenRate = staminaRegenRate;

            //standing still OR hiding regenerates stamina much faster
            if (!isMoving || IsHiding)
            {
                currentRegenRate *= standingRegenMultiplier;
            }

            currentStamina += currentRegenRate * Time.deltaTime;

            currentStamina = Mathf.Clamp(
                currentStamina,
                0f,
                maxStamina
            );
        }
    }

    public void DrainStamina(float amount)
    {
        currentStamina -= amount;

        currentStamina = Mathf.Clamp(
            currentStamina,
            0f,
            maxStamina
        );

        regenTimer = regenDelay;
    }
}