using Godot;
using System;

public class Warp : Godot.Area2D
{
    [Export]
    int warpNumber = 0;
    [Export]
    string targetMap = "";
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
        if (!GetNode<BetweenMaps>("/root/BetweenMaps").movingToNewMap)
        {
            if (body.IsInGroup("Player"))
            {
                body.Call("MoveToNewMap", this, targetMap, targetExitNumber);
            }
        }
    }
}
