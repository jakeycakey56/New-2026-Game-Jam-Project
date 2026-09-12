using UnityEngine;

public class TollBoothInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    private bool isTalking = false;

    private void Update()
    {
        // Start talking
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isTalking)
        {
            StartDialogue();
        }

        // Temporary way to end dialogue
        if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            EndDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered toll booth interaction range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left toll booth interaction range.");
        }
    }

    private void StartDialogue()
    {
        isTalking = true;

        Debug.Log("Talking to toll booth NPC.");
        Debug.Log("Game paused. Press Space to end dialogue.");

        Time.timeScale = 0f;
    }

    private void EndDialogue()
    {
        isTalking = false;

        Debug.Log("Dialogue ended.");

        Time.timeScale = 1f;
    }
}