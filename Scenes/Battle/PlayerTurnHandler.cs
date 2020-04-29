using Godot;
using System;

public class PlayerTurnHandler : Node
{
    Creature player;
    public override void _Ready()
    {
        player = (Creature) GetParent();  
    }

    public void SetupTurnHandler()
    {
        Button button = GetNode<Button>("./CanvasLayer/Attack");
        button.Connect("pressed", this, "DoAttack");
    }

    private void DoAttack() //change to DoAttack or something, match the button name not the method name
    {
        Creature enemy = null;
        foreach (Creature creature in GetTree().GetNodesInGroup("AliveInBattle"))
        {
            if (creature.CreatureName != player.CreatureName)
            {
                enemy = creature;
                break;
            }
        }
        player.BasicAttack(enemy);
        player.EmitSignal("TurnFinished");
        Button button = GetNode<Button>("./CanvasLayer/Attack");
        button.Disconnect("pressed", this, "DoAttack");
    }
}
