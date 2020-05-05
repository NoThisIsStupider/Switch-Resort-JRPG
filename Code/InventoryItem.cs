using System;
using System.Collections.Generic;

public class InventoryItem
{
    public string itemName;
    public string description; 
    public InventoryItem(string name, string description)
    {
        itemName = name;
        this.description = description;
    }

    public void Use()
    {

    }

    public static List<InventoryItem> TestItems()
    {
        List<InventoryItem> items = new List<InventoryItem>();
        items.Add(new InventoryItem("Apple", "Restores 10HP"));
        items.Add(new InventoryItem("Sword", "Slices and dices"));
        items.Add(new InventoryItem("You fell for it fool", "Thunder cross split attack"));
        return items;
    }
}
