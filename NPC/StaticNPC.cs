using Godot;
using System;
using System.Collections.Generic;

public class StaticNPC : StaticBody2D, NPC
{
    [Export]
    List<string> messages = new List<string>();
    Textbox textbox;
    AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        AddToGroup("NPC");

        textbox = GetNode<Textbox>("/root/Textbox");
        animationPlayer = GetNode<AnimationPlayer>("./AnimationPlayer");
    }

    //this method is responsible for both updating the animation and displaying the dialogue
    public async void Interact(Vector2 interactionNormal)
    {
        float angle = Mathf.Rad2Deg(Mathf.Atan2(interactionNormal.x, -interactionNormal.y));
        angle = (angle < 0) ? 360 + angle : angle;
        if (angle > 315 || angle < 45)
        {
            animationPlayer.Play("FaceUp");
        }
        else if (angle >= 45 && angle <= 135)
        {
            animationPlayer.Play("FaceRight");
        }
        else if (angle > 135 && angle < 225)
        {
            animationPlayer.Play("FaceDown");
        }
        else if (angle >= 225 && angle <= 315)
        {
            animationPlayer.Play("FaceLeft");
        }
        await ToSignal(GetTree(), "physics_frame"); //wait a frame before opening the textbox so the sprite can update
        textbox.SetupTextbox(messages);
    }
}
