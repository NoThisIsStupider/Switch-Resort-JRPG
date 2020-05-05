using Godot;
using System;

public enum EffectTypes 
{
    FadeIn,
    FadeOut, 
    BattleAnim1
}

public class ScreenEffects : CanvasLayer
{
    //from, to, and weight are used in the lerp for the fading, they are kept generic so they can be used for fadein and fadeout
    float from = 0;
    float to = 1;
    float weight = 0;
    float effectLength = 1;

    ColorRect colorRect;

    public override void _Ready()
    {
        colorRect = GetNode<ColorRect>("./ColorRect");

        //GetNode("/root/Events").Connect("FadeScreen", this, "BeginFade");
        SetProcess(false);
    }

    public override void _Process(float delta)
    {
        weight += delta / effectLength;

        (colorRect.Material as ShaderMaterial).SetShaderParam("effectTime", Mathf.Lerp(from, to, weight));

        if (weight >= 1)
        {
            SetProcess(false);
        }
    }

    //a fadeLength variable should be added since right now the fade can only be for 1 second
    public void BeginEffect(EffectTypes effectType, float effectLength)
    {
        this.effectLength = effectLength;
        switch (effectType)
        {
            case EffectTypes.FadeIn:
                from = 1;
                to = 0;
                break;
            case EffectTypes.FadeOut:
                from = 0;
                to = 1;
                break;
        }
        weight = 0;
        SetProcess(true);
    }
}
