using Godot;
using System;

public class PlayerHealthBar : ProgressBar
{
    Creature battlePlayer = null;
    public override void _Ready()
    {
        battlePlayer = (Creature) GetParent();
        MaxValue = battlePlayer.MaxHealth;
        battlePlayer.Connect("HealthChanged", this, "UpdateHealthBar");
    }

    public void UpdateHealthBar(int newValue)
    {
        Value = newValue;
    }
}
