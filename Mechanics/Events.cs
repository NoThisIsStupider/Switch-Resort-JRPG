using Godot;
using System;

//This singleton is used to hold signals that need to be used in a publish-subscribe fashion

public class Events : Node
{
    [Signal]
    delegate void ShowMessage(string message); //Used for Textboxes to show a simple message
    //perhaps add more signals for more ways of calling show message?
    //idea: one signal for messages can have a delegate passed that uses yield to pass each successive string or command to the message box, could be pretty interesting and flexible
}