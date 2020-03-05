using Godot;
using System;

public class Textbox : MarginContainer
{

    public override void _Ready()
    {
        GetNode("/root/Events").Connect("ShowMessage", this, "SetupTextbox");
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_accept"))
        {
            ExitTextbox();
        }
    }

    public void SetupTextbox(string message)
    {
        Label label = (Label) GetNode("./Panel/Label");
        label.Text = message;
        this.Visible = true;
        GetTree().Paused = true;
    }

    public void ExitTextbox()
    {
        this.Visible = false;
        GetTree().Paused = false;
    }
}
