using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Money")]
    [SerializeField] private int money = 0;

    [Header("Ticket")]
    [SerializeField] private bool hasTicket = false;

    //this keeps track of the collectible items the player is carrying
    private List<CollectibleItem> collectedItems =
        new List<CollectibleItem>();

    //lets other scripts check how much money the player has
    public int Money => money;

    //lets other scripts check if the player has a ticket
    public bool HasTicket => hasTicket;

    //lets other scripts see the items we're currently carrying
    public IReadOnlyList<CollectibleItem> CollectedItems =>
        collectedItems;

    public void AddItem(CollectibleItem item)
    {
        if (item == null)
            return;

        collectedItems.Add(item);

        Debug.Log(
            "Collected " + item.ItemName +
            " worth $" + item.Value
        );
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        money += amount;

        Debug.Log(
            "Player now has $" + money
        );
    }

    public bool SpendMoney(int amount)
    {
        //don't allow invalid purchases
        if (amount <= 0)
            return false;

        //not enough money
        if (money < amount)
        {
            Debug.Log(
                "Not enough money."
            );

            return false;
        }

        money -= amount;

        Debug.Log(
            "Spent $" + amount +
            ". Remaining money: $" + money
        );

        return true;
    }

    public void GiveTicket()
    {
        hasTicket = true;

        Debug.Log(
            "Player received a bus ticket."
        );
    }
}