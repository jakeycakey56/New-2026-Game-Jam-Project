using UnityEngine;

public class EnemySpawnDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameClock gameClock;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnCheckInterval = 5f;

    [Header("Spawn Chance")]
    [SerializeField] private float baseSpawnChance = 0.30f;
    [SerializeField] private float chanceIncreasePerHour = 0.10f;

    private float spawnTimer = 0f;

    private void Update()
    {
        //Don't run spawn checks while the clock is paused
        if (gameClock == null || gameClock.IsPaused)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnCheckInterval)
        {
            spawnTimer -= spawnCheckInterval;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        if (enemySpawner == null)
        {
            Debug.LogWarning("EnemySpawner reference is missing.");
            return;
        }

        int hoursPassed = gameClock.HoursPassed;

        float spawnChance =
            baseSpawnChance +
            (hoursPassed * chanceIncreasePerHour);

        //make sure the chance never goes above 100%
        spawnChance = Mathf.Clamp01(spawnChance);

        float roll = Random.value;

        Debug.Log(
            "Enemy spawn roll: " + (roll * 100f) +
            "% | Chance: " + (spawnChance * 100f) + "%"
        );

        if (roll <= spawnChance)
        {
            Debug.Log("Enemy spawn roll succeeded.");
            enemySpawner.SpawnEnemy();
        }
        else
        {
            Debug.Log("Enemy spawn roll failed.");
        }
    }
}