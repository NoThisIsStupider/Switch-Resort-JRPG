using Godot;
using System;
using System.Collections.Generic;

//This singleton is used to hold signals that need to be used in a publish-subscribe fashion

public class Events : Node
{    
    [Signal]
    //The player uses this to know when to change between maps
    delegate void WarpEntered(Node2D entrance, string targetMapScenePath, int exitNumber); 

    //signal used by BetweenMaps.cs to call the players method to setup for the new map (maybe adjust the naming here?)
    [Signal]
    delegate void NewMapEntered(Node2D entranceNode, Vector2 playerMoveDir);

    [Signal]
    delegate void FadeScreen(bool isFadeout, float fadeTime);
}