using Godot;
using System;

public class Warp : Godot.Area2D
{
    [Export]
    int entranceNumber = 0;
    [Export]
    string targetMapScenePath = "";
    [Export]
    int targetExit = -1;
    public override void _Ready()
    {
        Connect("body_entered", this, "OnBodyEntered");
        AddToGroup("Warps");
    }

    public void OnBodyEntered(Node body)
    {
        if (!(bool) GetNode("/root/BetweenMaps").Get("movingToNewMap"))
        {
            GetNode("/root/Events").EmitSignal("WarpEntered", this, targetMapScenePath, targetExit);
        }
    }
}
