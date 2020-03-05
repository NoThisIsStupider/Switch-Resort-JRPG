using Godot;
using System;

public class Area2D : Godot.Area2D
{
    
    public override void _Ready()
    {
        Node textbox = GetNode("/root/World/OverworldPlayer/CanvasLayer/Textbox");
        Connect("body_entered", this, "Test");
    }

    public void Test(Node body)
    {
        GetNode("/root/Events").EmitSignal("ShowMessage", "Hello World!");
    }
}