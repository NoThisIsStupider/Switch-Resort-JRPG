using Godot;
using System;

//this singleton is used to enable transferring data between to a new map, like the player's position and such

public class BetweenMaps : Node
{
    int exitNumber;
    Vector2 storedPlayerMoveDirection;
    bool movingToNewMap = false;

    private void SetupNewMap()
    {
        Node exit = (Node) (GetTree().CurrentScene.Get("Warps") as Godot.Collections.Dictionary)[exitNumber];
        GetNode("/root/Events").EmitSignal("NewMapEntered", exit, storedPlayerMoveDirection);
    }

    public void PrepareForMapChange(int exitNumber, Vector2 storedPlayerMoveDirection)
    {
        this.exitNumber = exitNumber;
        this.storedPlayerMoveDirection = storedPlayerMoveDirection;
        movingToNewMap = true;
    }

    public void MapChangeFinished()
    {
        movingToNewMap = false;
    }
}