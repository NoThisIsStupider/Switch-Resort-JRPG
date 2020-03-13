using Godot;
using System;
using System.Collections.Generic;

//TODO here:
//Add in a textbox opening/closing animation, use the ysquish property of the shader to implement 

enum TextboxStates
{
    WaitingForAdvanceInput,
    TextCrawl,
    OpenOrClose
}

public class Textbox : Control
{
    TextboxStates currentState;
    //Text Crawl Related
    bool isCrawling = false;
    const float TEXT_CRAWL_SPEED = 50f;
    //Multiple Message Display Related
    int currentMessageIndex;
    List<string> storedMessages;
    //Open or Close Related
    float from;
    float to;
    float weight;
    const float ANIMATION_LENGTH = 0.2f;
    //Child nodes
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
        switch (currentState)
        {
            case TextboxStates.WaitingForAdvanceInput:
                if (Input.IsActionJustPressed("interact"))
                {
                    currentMessageIndex++;
                    if (currentMessageIndex < storedMessages.Count)
                    {
                        SetupTextCrawl();
                        label.Text = storedMessages[currentMessageIndex];
                    }
                    else
                    {
                        SetupOpenOrClose(false);   
                    }
                }
                break;
            case TextboxStates.TextCrawl:
                if (Input.IsActionJustPressed("interact"))
                {
                    label.PercentVisible = 1;
                }
                label.PercentVisible += (delta / storedMessages[currentMessageIndex].Length) * TEXT_CRAWL_SPEED;
                if (isTextCrawlReachedEnd())
                {
                    isCrawling = false;
                    currentState = TextboxStates.WaitingForAdvanceInput;
                }
                break;
            case TextboxStates.OpenOrClose:
                weight += delta / ANIMATION_LENGTH;
                if (weight >= 1)
                {
                    (background.Material as ShaderMaterial).SetShaderParam("ysquish", 1);
                    if (from == 0) //did the textbox start closed?
                    {
                        SetupTextCrawl();
                    }
                    else 
                    {
                        ExitTextbox();
                    }
                    break;
                }
                (background.Material as ShaderMaterial).SetShaderParam("ysquish", Mathf.Lerp(from, to, weight));
                break;
        }
    }

    //this method prepares the textbox to display a list of messages
    public void SetupTextbox(List<string> messages)
    {
        if (IsProcessing()) //a textbox is open already
        {
            return;
        }

        SetupOpenOrClose(true);

        currentMessageIndex = 0; //initialize this so it shows all the messages
        storedMessages = new List<string>(messages); //the new List creation is used to avoid a crash

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
        
        //return the textbox to a dormant state and unpause the rest of the game
        setTextboxVisibility(false);
        GetTree().Paused = false;
        SetProcess(false);

        Input.ActionRelease("interact"); //release the interact input so the textbox doesn't manage to reopen itself immediately
    
        //setup the close animation
        currentState = TextboxStates.OpenOrClose;
    }

    private void setTextboxVisibility(bool visible)
    {
        background.Visible = visible;
        label.Visible = visible;
    }

    private bool isTextCrawlReachedEnd()
    {
        return label.PercentVisible >= 1;
    }

    private void SetupTextCrawl()
    {
        label.VisibleCharacters = 0;
        isCrawling = true;
        currentState = TextboxStates.TextCrawl;
    }

    private void SetupOpenOrClose(bool isOpen)
    {
        label.VisibleCharacters = 0; //so the label doesn't keep displaying during this
        if (isOpen)
        {
            from = 0;
            to = 1;
        }
        else 
        {
            from = 1;
            to = 0;
        }
        weight = 0;
        currentState = TextboxStates.OpenOrClose;
    }
}
