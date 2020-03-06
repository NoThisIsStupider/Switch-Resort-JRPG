using Godot;
using System;

public class Textbox : MarginContainer
{
    Timer timer;
    bool isMessageOpen = false;
    public override void _Ready()
    {
        GetNode("/root/Events").Connect("ShowMessage", this, "SetupTextbox"); //setup signal so textbox can recieve messages to display
        timer = GetNode<Timer>("./Timer");
        timer.OneShot = true;
    }

    public override void _Process(float delta)
    {
        if (isMessageOpen && timer.IsStopped() && Input.IsActionJustPressed("ui_accept")) //used current for dismissing the textbox, very quick and dirty, replace with more complex logic later (like checking that the textbox is even open first)
        {
            ExitTextbox();
        }
    }

    //Both of these are quick and dirty methods, the textbox needs lots of revisions to be in a good state
    public void SetupTextbox(string message)
    {
        if (isMessageOpen)
        {
            return;
        }
        Label label = (Label) GetNode("./Panel/Label");
        label.Text = message;
        this.Visible = true;
        GetTree().Paused = true;
        isMessageOpen = true;
        timer.Start(0.1f);
    }

    public void ExitTextbox()
    {
        this.Visible = false;
        GetTree().Paused = false; 
        isMessageOpen = false;
    }
}
