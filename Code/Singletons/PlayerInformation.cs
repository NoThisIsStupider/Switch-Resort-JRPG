using Godot;
using System;
using System.Collections.Generic;

public class PlayerInformation : Node
{
    public List<InventoryItem> inventory = new List<InventoryItem>();
    public override void _Ready()
    {
        inventory = InventoryItem.TestItems();
    }
}
