using Godot;
using System;

public class Area2D : Godot.Area2D
{
    [Export]
    string targetMapScenePath = "";
    [Export]
    int targetEntrance = -1;
    public override void _Ready()
    {
        Connect("body_entered", this, "Test");
    }

    public void Test(Node body)
    {
        GetNode("/root/Events").EmitSignal("LoadZoneEntered", targetMapScenePath, targetEntrance, Position);
    }
}
