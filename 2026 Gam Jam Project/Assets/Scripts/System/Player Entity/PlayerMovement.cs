using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent PlayerAgent;
    private PlayerStamina playerStamina;

    public Camera PlayerCamera;

    public float MoveSpeed = 10f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float staminaDrainPerSecond = 25f;

    [SerializeField] float SampleDistance = .5f;
    [SerializeField] LayerMask GroundLayer;

    public static event System.Action<Vector3> OnGroundTouch;

    void Start()
    {
        PlayerAgent = GetComponent<NavMeshAgent>();
        playerStamina = GetComponent<PlayerStamina>();

        //start the NavMeshAgent at normal walking speed
        PlayerAgent.speed = MoveSpeed;
    }

    void Update()
    {
        HandleSprint();

        if (Input.GetMouseButton(0)) //Changed this to also allow click+hold movement
        {
            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GroundLayer))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, SampleDistance, NavMesh.AllAreas))
                {
                    PlayerAgent.SetDestination(navMeshHit.position);

                    OnGroundTouch?.Invoke(navMeshHit.position);
                }
                else
                {
                    Debug.Log("You clicked outside of the walkable Area");
                }
            }
        }
    }

    private void HandleSprint()
    {
        //check whether the player is actually moving
        bool isMoving = PlayerAgent.velocity.sqrMagnitude > 0.01f;

        //only sprint and drain stamina while moving
        if (Input.GetKey(KeyCode.LeftShift) &&
            playerStamina != null &&
            playerStamina.HasStamina &&
            isMoving)
        {
            PlayerAgent.speed = sprintSpeed;

            //drain stamina only while actively sprinting
            playerStamina.DrainStamina(staminaDrainPerSecond * Time.deltaTime);
        }
        else
        {
            //return to normal movement speed when not sprinting
            PlayerAgent.speed = MoveSpeed;
        }
    }
}