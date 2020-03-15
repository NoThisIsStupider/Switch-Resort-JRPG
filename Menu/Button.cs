using Godot;
using System;

public class Button : Godot.Button
{
    public override void _Ready()
    {
        Connect("pressed", this, "OnPress");
    }

    public void OnPress()
    {
        AudioStreamPlayer backgroundMusic = GetNode<AudioStreamPlayer>("/root/BackgroundMusic");
        GD.Print(backgroundMusic.VolumeDb);
        if (!backgroundMusic.Playing)
        {
            backgroundMusic.VolumeDb = 0;
        }
        else 
        {
            //backgroundMusic.Stop();
        }
    }
}
