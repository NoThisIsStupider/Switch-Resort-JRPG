using Godot;
using System;

public class OverworldMap : Node2D
{
    public Godot.Collections.Dictionary Warps = new Godot.Collections.Dictionary();

    public override void _Ready()
    {
        UpdateWarps();
        if ((bool) GetNode("/root/BetweenMaps").Get("movingToNewMap"))
        {
            GetNode("/root/BetweenMaps").Call("SetupNewMap");
        }
    }

    public void UpdateWarps()
    {
        foreach (Node warp in GetTree().GetNodesInGroup("Warps"))
        {
            Warps[warp.Get("entranceNumber")] = warp;
        }
    }
}
