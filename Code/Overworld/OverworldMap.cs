using Godot;
using System;

public class OverworldMap : Node2D
{
	public override void _Ready()
	{
		if ((bool) GetNode("/root/BetweenMaps").Get("movingToNewMap"))
		{
			GetNode("/root/BetweenMaps").Call("SetupNewMap");
		}
	}
}
