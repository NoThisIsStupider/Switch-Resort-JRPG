using Godot;
using System;

public class Warp : Godot.Area2D
{
    [Export]
    int warpNumber = 0;
    [Export]
    string targetMapScenePath = "";
    [Export]
    int targetExitNumber = -1;
    [Export]
    public Vector2 outDirectionNormalized; //is normalized at runtime
    public override void _Ready()
    {
        Connect("body_entered", this, "OnBodyEntered");
        AddToGroup("Warps");

        outDirectionNormalized = outDirectionNormalized.Normalized();
    }

    public void OnBodyEntered(Node body)
    {
        if (!(bool) GetNode("/root/BetweenMaps").Get("movingToNewMap"))
        {
            //GetNode("/root/Events").EmitSignal("WarpEntered", this, targetMapScenePath, targetExitNumber);
            if (body.IsInGroup("Player"))
            {
                body.Call("MoveToNewMap", this, targetMapScenePath, targetExitNumber);
            }
        }
    }
}
