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

    [Header("Facing Settings")]
    [SerializeField] private float rotationSpeed = 720f; //how quickly the player turns to face the mouse

    [SerializeField] float SampleDistance = .5f;
    [SerializeField] LayerMask GroundLayer;

    public static event System.Action<Vector3> OnGroundTouch;

    void Start()
    {
        PlayerAgent = GetComponent<NavMeshAgent>();
        playerStamina = GetComponent<PlayerStamina>();

        //we're handling the player's rotation ourselves now
        //this lets the player face the mouse instead of wherever the NavMeshAgent is moving
        PlayerAgent.updateRotation = false;

        //start the NavMeshAgent at normal walking speed
        PlayerAgent.speed = MoveSpeed;
    }

    void Update()
    {
        HandleSprint();

        //make the player face wherever the mouse is pointing
        //this is separate from movement, so we can walk one way while looking another
        FaceMouseCursor();

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

    //handles making the player face the mouse
    private void FaceMouseCursor()
    {
        //shoot a ray from the camera through wherever the mouse is on the screen
        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GroundLayer))
        {
            //figure out which direction the mouse is from the player
            Vector3 direction = hit.point - transform.position;

            //we don't want the player trying to look up/down at the ground
            //so completely ignore the Y difference
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                //rotate toward the mouse instead of instantly snapping to it
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
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