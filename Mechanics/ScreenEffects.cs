using Godot;
using System;

public class ScreenEffects : ColorRect
{
    //from, to, and weight are used in the lerp for the fading, they are kept generic so they can be used for fadein and fadeout
    float from = 0;
    float to = 1;
    float weight = 0;
    float fadeTime = 1;

    public override void _Ready()
    {
        GetNode("/root/Events").Connect("FadeScreen", this, "BeginFade");
        SetProcess(false);
    }

    public override void _Process(float delta)
    {
        weight += delta / fadeTime;

        (Material as ShaderMaterial).SetShaderParam("fadeLevel", Mathf.Lerp(from, to, weight));

        if (weight >= 1)
        {
            SetProcess(false);
        }
    }

    //a fadeLength variable should be added since right now the fade can only be for 1 second
    public void BeginFade(bool isFadeout, float fadeTime)
    {
        this.fadeTime = fadeTime;
        if (isFadeout)
        {
            from = 0;
            to = 1;
        }
        else 
        {
            from = 1;
            to = 0;
        }
        weight = 0;
        SetProcess(true);
    }
}
