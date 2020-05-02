using Godot; 
using System;

class ManualSceneSwitcher : Node 
{
    Node overworld;
    Node battle;
    public override void _Process(float delta)
    {
        if (Input.IsKeyPressed((int) Godot.KeyList.B))
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
    }
}