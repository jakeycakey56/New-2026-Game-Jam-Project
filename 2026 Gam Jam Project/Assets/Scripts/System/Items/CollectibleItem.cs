using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    //Different categories of collectible items
    public enum ItemType
    {
        Junk,
        Special
    }

    [Header("Item Settings")]
    [SerializeField] private string itemName = "Scrap";
    [SerializeField] private int value = 5;
    [SerializeField] private ItemType itemType = ItemType.Junk;

    private bool playerInRange = false;
    private PlayerInventory playerInventory;

    //lets PlayerInventory and other scripts read the item's information
    public string ItemName => itemName;
    public int Value => value;
    public ItemType Type => itemType;

    private void Update()
    {
        if (!playerInRange)
            return;

        //Pick up the item when the player presses E
        if (Input.GetKeyDown(KeyCode.E))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        if (playerInventory == null)
            return;

        //add this item to the player's inventory
        playerInventory.AddItem(this);

        //remove the physical item from the world
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        //get the inventory from the player that entered our trigger
        playerInventory = other.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        playerInventory = null;
    }
}