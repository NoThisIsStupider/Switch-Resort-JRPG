using Godot;
using System;
using System.Collections.Generic;

public class Textbox : Control
{
    int currentMessageIndex;
    List<string> storedMessages;
    Control background;
    Label label;

    public override void _Ready()
    {
        background = GetNode<Control>("./CanvasLayer/Background");
        label = GetNode<Label>("./CanvasLayer/Label");

        setTextboxVisibility(false); //keep the textbox invisible at first
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
    public void SetupTextbox(List<string> messages)
    {
        if (IsProcessing()) //a textbox is open already
        {
            return;
        }
        
        currentMessageIndex = 0; //initialize this so it shows all the messages
        storedMessages = new List<string>(messages);

        //make the textbox visible and place the text inside
        label.Text = storedMessages[0];
        setTextboxVisibility(true);
        
        //pause everything else and enable processing for the textbox (to check for a close input)
        GetTree().Paused = true;

        Input.ActionRelease("interact"); //release the interact input so the textbox doesn't instantly advance the first frame it's shown
        SetProcess(true);
    }

    public void ExitTextbox()
    {
        //no need to clear the label here since it will be overwritten next time it is used
        setTextboxVisibility(false);
        GetTree().Paused = false;
        Input.ActionRelease("interact"); //release the interact input so the textbox doesn't manage to reopen itself immediately
        SetProcess(false);
    }

    private void setTextboxVisibility(bool visible)
    {
        background.Visible = visible;
        label.Visible = visible;
    }
}
