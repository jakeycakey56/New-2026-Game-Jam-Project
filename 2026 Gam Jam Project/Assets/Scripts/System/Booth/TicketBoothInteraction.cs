using UnityEngine;

public class TicketBoothInteraction : MonoBehaviour
{
    private bool playerInRange = false;

    private void Start()
    {
        Debug.Log("Ticket Booth Interaction script started.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed.");

            if (playerInRange)
            {
                Interact();
            }
            else
            {
                Debug.Log("E pressed, but player is NOT in range.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the Ticket Booth trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered Ticket Booth interaction range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Something exited the Ticket Booth trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left Ticket Booth interaction range.");
        }
    }

    private void Interact()
    {
        Debug.Log("INTERACTING WITH TICKET BOOTH!");
    }
}