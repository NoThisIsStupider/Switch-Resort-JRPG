using Godot;
using System;

/* Idea for selecting an enemy to attack:
 * Basically, since Node2D allows UI selection to continue working, code a custom ui element (probably using base button) that handles selecting the enemy 
 * could be fun to do an outline around the enemy sprite to show they are selected, write a shader to test that
 */

public class PlayerTurnHandler : Node
{
    Creature player;
    Textbox textbox;

    public override void _Ready()
    {
        player = (Creature) GetParent();  
        textbox = GetNode<Textbox>("/root/Textbox");
    }

    public void SetupTurnHandler()
    {
        Button button = GetNode<Button>("./CanvasLayer/Control/Attack");
        button.Connect("pressed", this, "DoAttack");
        button.GrabFocus();
    }

    //Add some textboxes to this stuff
    private void DoAttack()
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
        Button button = GetNode<Button>("./CanvasLayer/Control/Attack");
        button.Disconnect("pressed", this, "DoAttack");
    }

    private void DoRun()
    {

    }
}
