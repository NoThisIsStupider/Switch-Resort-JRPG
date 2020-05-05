using Godot;
using System;

//This sets the rect size uniform in the textbox shader, this only needs to be set for applications of the textbox background that need squash and stretch

public class TextboxBackground : Control
{
    public override void _Ready()
    {
        (Material as ShaderMaterial).SetShaderParam("size_y", RectSize.y);
    }
}
