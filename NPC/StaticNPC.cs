using Godot;
using System;
using System.Collections.Generic;

public class StaticNPC : StaticBody2D, NPC
{
    [Export]
    private List<string> messages = new List<string>();
    private Textbox textbox;

    public override void _Ready()
    {
        AddToGroup("NPC");

        textbox = GetNode<Textbox>("/root/Textbox");
    }

    //this method is responsible for both updating the animation and displaying the dialogue
    public void Interact(Vector2 interactionNormal)
    {
        
        textbox.SetupTextbox(messages); //the new List creation is used to avoid a crash
    }
}
