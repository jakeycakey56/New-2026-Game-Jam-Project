using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent PlayerAgent;
    public Camera PlayerCamera;

    public float MoveSpeed = 10f;

    [SerializeField] float SampleDistance = .5f;
    [SerializeField] LayerMask GroundLayer;

    public static event System.Action<Vector3> OnGroundTouch;

    void Start()
    {
        PlayerAgent = GetComponent<NavMeshAgent>();


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
}
