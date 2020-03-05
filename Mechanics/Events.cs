using Godot;
using System;

public class Events : Node
{
    [Signal]
    delegate void ShowMessage(string message);
}