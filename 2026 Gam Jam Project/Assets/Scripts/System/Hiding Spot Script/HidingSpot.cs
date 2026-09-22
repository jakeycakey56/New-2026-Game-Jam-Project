using UnityEngine;
using UnityEngine.AI;

public class HidingSpot : MonoBehaviour
{
    [Header("Hiding Spot Points")]
    [SerializeField] private Transform sitPoint;
    [SerializeField] private Transform exitPoint;

    private PlayerMovement playerMovement;
    private PlayerGrab playerGrab;
    private PlayerStamina playerStamina;
    private NavMeshAgent playerAgent;

    private bool playerInRange = false;
    private bool playerIsHiding = false;

    private GameObject player;

    private void Update()
    {
        if (!playerInRange && !playerIsHiding)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!playerIsHiding)
            {
                //Prevent the player from hiding while grabbed
                if (playerGrab != null && playerGrab.IsGrabbed)
                    return;

                EnterHidingSpot();
            }
            else
            {
                ExitHidingSpot();
            }
        }
    }

    private void EnterHidingSpot()
    {
        if (player == null)
            return;

        playerIsHiding = true;

        //stop the player's current movement
        if (playerAgent != null)
        {
            playerAgent.ResetPath();
            playerAgent.isStopped = true;
        }

        //disable normal click-to-move while hiding
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        //snap the player onto the bench
        player.transform.position = sitPoint.position;
        player.transform.rotation = sitPoint.rotation;

        //tell stamina that the player is hiding/resting
        if (playerStamina != null)
        {
            playerStamina.IsHiding = true;
        }

        Debug.Log("Player entered hiding spot.");
    }

    private void ExitHidingSpot()
    {
        if (player == null)
            return;

        playerIsHiding = false;

        //move the player safely back in front of the bench
        player.transform.position = exitPoint.position;
        player.transform.rotation = exitPoint.rotation;

        if (playerAgent != null)
        {
            playerAgent.isStopped = false;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerStamina != null)
        {
            playerStamina.IsHiding = false;
        }

        Debug.Log("Player exited hiding spot.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        player = other.gameObject;

        playerMovement = player.GetComponent<PlayerMovement>();
        playerGrab = player.GetComponent<PlayerGrab>();
        playerStamina = player.GetComponent<PlayerStamina>();
        playerAgent = player.GetComponent<NavMeshAgent>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!playerIsHiding)
        {
            playerInRange = false;
            player = null;
        }
    }
}