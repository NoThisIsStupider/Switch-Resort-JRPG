using Godot;
using System;

public class OverworldPlayer : KinematicBody2D
{
    public override void _Ready()
    {
        Input.SetUseAccumulatedInput(false);
    }

    public override void _Process(float delta)
    {
        Vector2 input = new Vector2();
        input.x = Convert.ToInt32(Input.IsActionPressed("ui_right")) - Convert.ToInt32(Input.IsActionPressed("ui_left"));
        input.y = Convert.ToInt32(Input.IsActionPressed("ui_down")) - Convert.ToInt32(Input.IsActionPressed("ui_up"));
        this.MoveAndSlide(input.Normalized() * 100);
    }
}
