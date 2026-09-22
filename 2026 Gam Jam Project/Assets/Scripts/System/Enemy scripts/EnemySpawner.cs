using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    //lets us make multiple enemy entries in the Inspector
    [System.Serializable]
    public class EnemyVariant
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    [Header("Enemy Variants")]
    [SerializeField] private EnemyVariant[] enemyVariants;

    [Header("Spawn Rules")]
    [SerializeField] private float minimumPlayerDistance = 15f;
    [SerializeField] private int maxSpawnAttempts = 30;

    [Header("References")]
    [SerializeField] private Transform player;

    private void Update()
    {
        //Temporary testing button, remove later
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing.");
            return;
        }

        //pick which enemy we're going to spawn
        GameObject selectedEnemy = ChooseEnemyVariant();

        if (selectedEnemy == null)
        {
            Debug.LogWarning("No valid enemy variants are available to spawn.");
            return;
        }

        //find somewhere valid on the NavMesh
        if (TryGetValidSpawnPosition(out Vector3 spawnPosition))
        {
            Instantiate(
                selectedEnemy,
                spawnPosition,
                Quaternion.identity
            );
        }
        else
        {
            Debug.LogWarning(
                "Could not find a valid enemy spawn position."
            );
        }
    }

    private GameObject ChooseEnemyVariant()
    {
        if (enemyVariants == null || enemyVariants.Length == 0)
            return null;

        float totalWeight = 0f;

        //add up the weights of every usable enemy
        foreach (EnemyVariant variant in enemyVariants)
        {
            //empty slots are ignored
            if (variant.prefab == null)
                continue;

            //0 weight means this enemy can't currently spawn
            if (variant.weight <= 0f)
                continue;

            totalWeight += variant.weight;
        }

        if (totalWeight <= 0f)
            return null;

        //roll somewhere inside our total weight
        float randomRoll = Random.Range(0f, totalWeight);

        float currentWeight = 0f;

        //find which enemy our roll landed on
        foreach (EnemyVariant variant in enemyVariants)
        {
            if (variant.prefab == null)
                continue;

            if (variant.weight <= 0f)
                continue;

            currentWeight += variant.weight;

            if (randomRoll <= currentWeight)
            {
                return variant.prefab;
            }
        }

        return null;
    }

    private bool TryGetValidSpawnPosition(out Vector3 spawnPosition)
    {
        //get the triangles that make up the baked NavMesh
        NavMeshTriangulation navMesh =
            NavMesh.CalculateTriangulation();

        if (navMesh.indices.Length < 3)
        {
            spawnPosition = Vector3.zero;
            return false;
        }

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            //pick a random spot somewhere on the NavMesh
            Vector3 randomPoint =
                GetRandomPointOnNavMesh(navMesh);

            float distanceFromPlayer =
                Vector3.Distance(
                    randomPoint,
                    player.position
                );

            //don't let enemies spawn too close to the player
            if (distanceFromPlayer < minimumPlayerDistance)
                continue;

            spawnPosition = randomPoint;
            return true;
        }

        //System couldn't find a valid spot after all attempts
        spawnPosition = Vector3.zero;
        return false;
    }

    private Vector3 GetRandomPointOnNavMesh(
        NavMeshTriangulation navMesh)
    {
        //pick a random triangle from the NavMesh
        int triangleIndex =
            Random.Range(
                0,
                navMesh.indices.Length / 3
            ) * 3;

        Vector3 a =
            navMesh.vertices[
                navMesh.indices[triangleIndex]
            ];

        Vector3 b =
            navMesh.vertices[
                navMesh.indices[triangleIndex + 1]
            ];

        Vector3 c =
            navMesh.vertices[
                navMesh.indices[triangleIndex + 2]
            ];

        //Then pick a random point inside that triangle
        return GetRandomPointInTriangle(
            a,
            b,
            c
        );
    }

    private Vector3 GetRandomPointInTriangle(
        Vector3 a,
        Vector3 b,
        Vector3 c)
    {
        //math for getting an evenly random point inside a triangle
        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        return (1f - r1) * a
             + r1 * (1f - r2) * b
             + r1 * r2 * c;
    }
}