using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    public List<Item> items;
    [SerializeField] private int coins = 10; 
    public Dictionary<Item, int> itemQuantities;

    public Inventory()
    {
        items = new List<Item>();
        itemQuantities = new Dictionary<Item, int>();
    }

    public void AddItem(Item item, int quantity)
    {
        if (itemQuantities.ContainsKey(item))
        {
            itemQuantities[item] += quantity;
        }
        else
        {
            items.Add(item);
            itemQuantities[item] = quantity;
        }
    }

    public bool RemoveItem(Item item, int quantity)
    {
        if (itemQuantities.ContainsKey(item) && itemQuantities[item] >= quantity)
        {
            itemQuantities[item] -= quantity;
            if (itemQuantities[item] <= 0)
            {
                items.Remove(item);
                itemQuantities.Remove(item);
            }
            return true;
        }
        return false;
    }

    public int GetItemQuantity(Item item)
    {
        return itemQuantities.ContainsKey(item) ? itemQuantities[item] : 0;
    }

    // Method to get the current number of coins
    public int GetCoins()
    {
        return coins;
    }

    // Method to add coins
    public void AddCoins(int amount)
    {
        coins += amount;
    }

    // Method to remove coins
    public bool RemoveCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }
}
