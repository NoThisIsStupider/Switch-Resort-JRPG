using Godot;
using System;
using System.Collections.Generic;

//This singleton is used to hold signals that need to be used in a publish-subscribe fashion

public class Events : Node
{    
    [Signal]
    delegate void FadeScreen(bool isFadeout, float fadeTime); //try and murder this signal, move the fade thing to a singleton perhaps
}