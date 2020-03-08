using Godot;
using System;

//This singleton is used to hold signals that need to be used in a publish-subscribe fashion

public class Events : Node
{
    [Signal]
    //Used for Textboxes to show a simple message
    delegate void ShowMessage(string message); 
    //perhaps add more signals for more ways of calling show message?
    //idea: one signal for messages can have a delegate passed that uses yield to pass each successive string or command to the message box, could be pretty interesting and flexible
    
    [Signal]
    //The player uses this to know when to change between maps
    delegate void LoadZoneEntered(string targetMapScenePath, int targetEntrance, Vector2 thisEntrancePosition); 
    //could be good to have multiple transition types (eg door, outdoor path, etc), maybe make an enum in Events to keep track?

    [Signal]
    delegate void FadeScreen(bool isFadeout);
}