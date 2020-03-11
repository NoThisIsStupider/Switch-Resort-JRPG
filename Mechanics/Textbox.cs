using Godot;
using System;
using System.Collections.Generic;

public class Textbox : Control
{
    int currentMessageIndex;
    List<string> storedMessages;
    Control panel;
    Label label;

    public override void _Ready()
    {
        panel = GetNode<Control>("./CanvasLayer/Panel");
        label = panel.GetNode<Label>("./Label");

        changeTextboxVisibility(false); //keep the textbox invisible at first
        SetProcess(false); //don't execute _process every frame unless it is needed
        PauseMode = PauseModeEnum.Process; //make it so the textbox will keep working when everything else is paused
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("interact"))
        {
            currentMessageIndex++;
            if (currentMessageIndex < storedMessages.Count)
            {
                label.Text = storedMessages[currentMessageIndex];
            }
            else
            {
                ExitTextbox();
            }
            
        }
    }

    //this method prepares the textbox to display a list of messages
    public async void SetupTextbox(List<string> messages)
    {
        if (IsProcessing()) //a textbox is open already
        {
            return;
        }
        
        currentMessageIndex = 0; //initialize this so it shows all the messages
        storedMessages = new List<string>(messages);

        //make the textbox visible and place the text inside
        label.Text = storedMessages[0];
        changeTextboxVisibility(true);
        
        //pause everything else and enable processing for the textbox (to check for a close input)
        GetTree().Paused = true;

        //wait a frame before enabling process so that the input for opening the textbox is no longer "just" pressed
        await ToSignal(GetTree(), "physics_frame");
        SetProcess(true);
    }

    public void ExitTextbox()
    {
        //no need to clear the label here since it will be overwritten next time it is used
        changeTextboxVisibility(false);
        GetTree().Paused = false;
        SetProcess(false);
    }

    private void changeTextboxVisibility(bool visible)
    {
        panel.Visible = visible;
    }
}
