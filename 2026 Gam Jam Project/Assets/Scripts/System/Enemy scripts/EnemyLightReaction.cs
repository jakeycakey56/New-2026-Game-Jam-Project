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

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();

        //start every enemy at full light health
        currentLightHealth = maxLightHealth;
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

            if (enemyAI != null)
            {
                enemyAI.SetStunned(false);
            }

            Debug.Log("Enemy left IID light!");
        }
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

    private void Die()
    {
        Debug.Log("Enemy burned away!");

        //temporary death behavior
        //we can replace this with the actual burn-away effect later
        Destroy(gameObject);
    }
}