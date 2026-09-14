using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float searchTime = 3f;

    [Header("Line of Sight Settings")]
    [SerializeField] private LayerMask obstacleMask; //layers that should block the enemy's vision
    [SerializeField] private float eyeHeight = 1.5f; //how high off the ground the enemy "sees" from

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Grab Settings")]
    [SerializeField] private float grabRange = 1.5f;

    private NavMeshAgent agent;

    private PlayerStamina playerStamina;
    private PlayerGrab playerGrab;

    private Vector3 lastKnownPosition; //using this to allow the enemies to "search"
    private float searchTimer; //And this will be how long they search for
    private float patrolTimer; //how long the enemy waits before choosing another patrol location

    private enum EnemyState //these are the states available to an enemy in the script
    {
        Chasing,
        Searching,
        Patrolling
    }

    private EnemyState currentState = EnemyState.Patrolling; //enemies should start by wandering around instead of standing still

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); //this SHOULD grab the NavMeshAgent attached to the Shadow Monster game object
    }

    private void Start()
    {
        if (player != null)
        {
            playerStamina = player.GetComponent<PlayerStamina>();
            playerGrab = player.GetComponent<PlayerGrab>();
        }

        patrolTimer = 0f; //allows the enemy to immediately choose its first patrol location
    }

    private void Update()
    {
        if (player == null)
            return;

        //check if the player is currently hiding
        bool playerIsHiding =
            playerStamina != null &&
            playerStamina.IsHiding;

        //check if the enemy is close enough to grab the player
        //hidden players should NOT be able to be grabbed
        if (!playerIsHiding)
        {
            float distanceToPlayer = Vector3.Distance(
                transform.position,
                player.position
            );

            if (distanceToPlayer <= grabRange)
            {
                //only grab if the player currently allows it
                if (playerGrab != null && playerGrab.CanBeGrabbed)
                {
                    playerGrab.BeginGrab(gameObject);
                    AudioEvents.OnSFXRequested?.Invoke(SFXType.PlayerGrab);

                    //stop moving while holding the player
                    agent.ResetPath();

                    return;
                }
            }
        }

        //check if the player is both within detection range AND visible to the enemy
        if (CanSeePlayer())
        {
            currentState = EnemyState.Chasing;
            lastKnownPosition = player.position; //continue updating the player position while they remain visible
            searchTimer = searchTime; //reset search timer every game tick while the player is visible
        }
        else if (currentState == EnemyState.Chasing) //if the enemy loses sight of the player, switch to searching
        {
            currentState = EnemyState.Searching;
        }

        switch (currentState)
        {
            case EnemyState.Chasing:
                agent.SetDestination(player.position);
                break;

            case EnemyState.Searching:
                SearchForPlayer();
                break;

            case EnemyState.Patrolling:
                Patrol();
                break;
        }
    }

    private bool CanSeePlayer() //checks both distance and whether something is blocking the enemy's view of the player
    {
        //if the player is hiding, the enemy should treat them as invisible
        if (playerStamina != null && playerStamina.IsHiding)
            return false;

        //start the vision ray at roughly the enemy's eye level instead of its feet
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;

        //get the direction and distance from the enemy's eyes to the player
        Vector3 directionToPlayer = player.position - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        //if the player is outside the detection radius, they cannot be seen
        if (distanceToPlayer > detectionRange)
            return false;

        //cast a ray toward the player and check if an obstacle blocks it
        if (Physics.Raycast(
            eyePosition,
            directionToPlayer.normalized,
            distanceToPlayer,
            obstacleMask))
        {
            return false; //something is between the enemy and the player
        }

        //player is close enough and nothing is blocking the enemy's view
        return true;
    }

    private void SearchForPlayer() //go to the last known location, search for a while, then begin patrolling
    {
        agent.SetDestination(lastKnownPosition);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            searchTimer -= Time.deltaTime;

            if (searchTimer <= 0f)
            {
                currentState = EnemyState.Patrolling;
                patrolTimer = 0f; //lets the enemy immediately choose its first patrol location
            }
        }
    }

    private void Patrol() //pick random valid locations around the enemy and move between them
    {
        //if the enemy is still moving toward its patrol location, don't choose another one yet
        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            return;

        patrolTimer -= Time.deltaTime;

        //wait for a short time after reaching a patrol destination
        if (patrolTimer > 0f)
            return;

        //pick a random point somewhere around the enemy
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        //find the nearest valid position on the NavMesh
        if (NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            patrolRadius,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            patrolTimer = patrolWaitTime;
        }
    }
}