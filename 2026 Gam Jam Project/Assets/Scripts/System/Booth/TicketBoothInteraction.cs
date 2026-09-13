using UnityEngine;

public class TicketBoothInteraction : MonoBehaviour
{
    private enum BoothState
    {
        Closed,
        Menu,
        Talking,
        Shopping
    }

    [Header("UI")]
    [SerializeField] private GameObject boothMenuPanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject shopPanel;

    private bool playerInRange = false;
    private BoothState currentState = BoothState.Closed;

    private PlayerMovement playerMovement;

    private void Start()
    {
        //Make sure all booth UI starts hidden
        if (boothMenuPanel != null)
            boothMenuPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void Update()
    {
        //Open the booth interaction
        if (playerInRange &&
            currentState == BoothState.Closed &&
            Input.GetKeyDown(KeyCode.E))
        {
            OpenInteractionMenu();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        playerMovement =
            other.GetComponent<PlayerMovement>();

        Debug.Log(
            "Player entered ticket booth interaction range."
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        Debug.Log(
            "Player left ticket booth interaction range."
        );

        if (currentState != BoothState.Closed)
        {
            CloseBooth();
        }

        playerMovement = null;
    }

    private void OpenInteractionMenu()
    {
        currentState = BoothState.Menu;

        PauseGame();

        HideAllPanels();

        if (boothMenuPanel != null)
            boothMenuPanel.SetActive(true);

        Debug.Log(
            "Ticket booth menu opened."
        );
    }

    public void ChooseTalk()
    {
        if (currentState != BoothState.Menu)
            return;

        currentState = BoothState.Talking;

        HideAllPanels();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        Debug.Log(
            "Talking to ticket booth NPC."
        );
    }

    public void ChooseShop()
    {
        if (currentState != BoothState.Menu)
            return;

        currentState = BoothState.Shopping;

        HideAllPanels();

        if (shopPanel != null)
            shopPanel.SetActive(true);

        Debug.Log(
            "Ticket booth shop opened."
        );
    }

    public void ReturnToMenu()
    {
        if (currentState == BoothState.Closed)
            return;

        currentState = BoothState.Menu;

        HideAllPanels();

        if (boothMenuPanel != null)
            boothMenuPanel.SetActive(true);

        Debug.Log(
            "Returned to ticket booth menu."
        );
    }

    public void ChooseLeave()
    {
        if (currentState == BoothState.Closed)
            return;

        CloseBooth();
    }

    private void CloseBooth()
    {
        currentState = BoothState.Closed;

        HideAllPanels();

        ResumeGame();

        Debug.Log(
            "Left the ticket booth interaction."
        );
    }

    private void HideAllPanels()
    {
        if (boothMenuPanel != null)
            boothMenuPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void PauseGame()
    {
        //Stop player movement input
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        //Freeze the entire game world
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}