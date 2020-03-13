using Godot;
using System;

//this singleton is used to enable transferring data between to a new map, like the player's position and such

public class BetweenMaps : Node
{
    int exitNumber;
    Vector2 storedPlayerMoveDirection;
    Vector2 entranceInDirection;
    bool movingToNewMap = false;

    private void SetupNewMap()
    {
        Node exit = null;
        foreach (Node warp in GetTree().GetNodesInGroup("Warps"))
        {
            if ((int) warp.Get("warpNumber") == exitNumber)
            {
                exit = warp;
                break;
            }
        }
        if (exit == null)
        {
            throw new Exception($"Specified exit ({exitNumber}) was not found");
        }
        //kinda gross but since there is only 1 player this is a way to access it
        (GetTree().GetNodesInGroup("Player")[0] as Node).Call("OnMapEnter", exit, storedPlayerMoveDirection, entranceInDirection);
    }

    public void PrepareForMapChange(int exitNumber, Vector2 storedPlayerMoveDirection, Vector2 entranceOutDirection)
    {
        this.exitNumber = exitNumber;
        this.storedPlayerMoveDirection = storedPlayerMoveDirection;
        entranceInDirection = new Vector2(-entranceOutDirection.x, -entranceOutDirection.y);
        movingToNewMap = true;
    }

    public void MapChangeFinished()
    {
        movingToNewMap = false;
    }
}