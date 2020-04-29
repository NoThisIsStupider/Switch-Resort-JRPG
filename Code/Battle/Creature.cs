using Godot;
using System;
using System.Collections.Generic;

//This class is for an indiviual creature in a battle, could be an enemy or a player, works for both

/*
 * General Ideas for implementing the battle system: 
 * - Child node with no script on the Creature scene, this script defines the code for all the abilities and such, since each creature will be different 
 * - For the ability scripts, could be a good idea to make them not per enemy but a bit more generic, so there aren't a million code files (aka stuff like all slimes share one .cs even if different slimes use different attacks)
 * - The battle system should basically just make a turn order based on agility at the start of each turn, and run through it, allowing each creature to act on their turn (think dq11 3d battles)
 * - To enable sharing mechanics between ai and player, perhaps have the code that handles decision making under a separate note with an interface 
 * - When health changes, emit a signal with the change value so everything can update (the proper use for signals, cool) - DONE
 * - Might need some sort of "CreatureID" variable since there could be 2 enemys with the same name, so names won't work for lookups
 */

[Flags]
enum StatusEffects
{

}

class Creature : Node2D
{
    static RandomNumberGenerator rng = new RandomNumberGenerator();
    bool isAi = false; //if true, this creature is controlled by the computer, could be an ai ally, enemy, boss, or even something not really an enemy at all, like a cutscene battle

    PlayerTurnHandler playerTurnHandler;

    [Signal] delegate void TurnFinished();
    [Signal] delegate void HealthChanged();

    [Export] public string CreatureName { get; set; }
    //maybe move this gross stuff to a struct? and then temporary and regular can be their own instance of the struct? lose the getters though
    [Export] public int MaxHealth { get;set; }
    public int Health { get;set; }
    public int temporaryHealth;
    [Export] public int Attack { get;set; }
    public int temporaryAttack;
    public int Magic { get;set; }
    public int temporaryMagic;
    public int Defense { get;set; }
    public int temporaryDefense;
    public int Agility { get;set; }
    public int temporaryAgility;

    StatusEffects currentStatus;
    Dictionary<StatusEffects, float> statusResistances;

    public override void _Ready()
    {
        playerTurnHandler = GetNode<PlayerTurnHandler>("./PlayerTurnHandler");

        Health = MaxHealth;
        AddToGroup("AliveInBattle");
    }

    public void BasicAttack(Creature target)
    {
        target.TakeDamage(Attack);
    }

    public void TakeDamage(int attackStrength)
    {
        //there'll have to be logic for subtracting from temporary hp here as well
        Health -= Mathf.Max(attackStrength - Defense, 0); //Make sure you can never take negative damage
        EmitSignal("HealthChanged", Health);
    }

    public void Heal(int magicStrength)
    {
        Health += magicStrength + rng.RandiRange(0, Mathf.FloorToInt(magicStrength * 0.2f));
    }

    public void AttemptInflictStatus(StatusEffects statusToInflict)
    {
        if (statusResistances.ContainsKey(statusToInflict))
        {
            if (rng.Randf() > statusResistances[statusToInflict])
            {
                return;
            }
        }
        currentStatus = currentStatus | statusToInflict;
    }

    public void ForceInflictStatus(StatusEffects statusToInflict)
    {
        currentStatus = currentStatus | statusToInflict;
    }

    public void DoTurn() 
    {
        GD.Print($"{CreatureName} {Health}");
        if (CreatureName == "Enemy")
        {
            foreach (Creature creature in GetTree().GetNodesInGroup("AliveInBattle"))
            {
                if (creature.CreatureName != CreatureName)
                {
                    BasicAttack(creature);
                    break;
                }
            }
        }
        else
        {
            playerTurnHandler.SetupTurnHandler();
        }
    }
}