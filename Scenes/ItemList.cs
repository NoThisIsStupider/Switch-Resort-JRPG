using Godot;
using System;
using System.Collections.Generic;

public class ItemList : Godot.ItemList
{
    public override void _Ready()
    {
        //AddItem("1");
        PopulateInventory();
        GrabFocus();
        Connect("item_activated", this, "OnActivate");
        Connect("item_selected", this, "OnSelect");
        Select(0);
        EmitSignal("item_selected", 0);
    }

    private void PopulateInventory()
    {
        Clear();
        foreach (InventoryItem item in (GetNode<PlayerInformation>("/root/PlayerInformation").inventory))
        {
            AddItem(item.itemName);
        }
    }

    private void OnActivate(int index)
    {
        GD.Print($"{index} activated.");
        GetNode<PopupMenu>("./PopupMenu").Popup_();
    }

    private void OnSelect(int index)
    {
        GetNode<Label>("../../PanelContainer/Label").Text = $"{GetNode<PlayerInformation>("/root/PlayerInformation").inventory[index].description}";
        GD.Print($"{index} selected.");
    }
}
