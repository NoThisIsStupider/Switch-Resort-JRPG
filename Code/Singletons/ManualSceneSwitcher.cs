using Godot; 
using System;
using System.Collections.Generic;

// The current implementation of this is just for quick testing, a far cleaner version should be written later on (the way it works making it cleaner should be very easy)

// The way a better version would work is via a stack, you push an old scene onto the stack, and then are using a new scene, and then you can either discard the new one and go back to the old, or nest another scene switch (eg going multiple levels down in a menu)

class ManualSceneSwitcher : Node 
{
    Stack<Node> scenes; //unused for now
    Node overworld;
    Node battle;
    Node menu; 
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("open_battle"))
        {
            if (battle == null)
            {
                GD.Print("Move to battle");
                battle = (ResourceLoader.Load("res://Scenes/Battle/Battle.tscn") as PackedScene).Instance();
                GetTree().Root.AddChild(battle);
                overworld = GetTree().CurrentScene;
                GetTree().Root.RemoveChild(overworld);
                GetTree().CurrentScene = battle;
            }
            else 
            {
                GD.Print("Move to overworld");
                GetTree().Root.AddChild(overworld);
                GetTree().CurrentScene = overworld;
                GetTree().Root.RemoveChild(battle);
                battle = null;
            }
        }
        if (Input.IsActionJustPressed("open_menu"))
        {
            if (menu == null)
            {
                GD.Print("Move to menu");
                menu = (ResourceLoader.Load("res://Scenes/InventoryScreen.tscn") as PackedScene).Instance();
                GetTree().Root.AddChild(menu);
                overworld = GetTree().CurrentScene;
                GetTree().Root.RemoveChild(overworld);
                GetTree().CurrentScene = menu;
            }
            else 
            {
                GD.Print("Move to overworld");
                GetTree().Root.AddChild(overworld);
                GetTree().CurrentScene = overworld;
                GetTree().Root.RemoveChild(menu);
                menu = null;
            }
        }
        
    }
}