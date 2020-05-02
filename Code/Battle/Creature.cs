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
    [Signal] delegate void CreateMessage(string message);

    [Export] public string CreatureName { get; set; }

    //practical values include temporary stuff
    [Export] public int MaxHealth { get;set; }
    public int MaxHealthPractical { get { return MaxHealth + maxHealthTemporary; }} //Current Health can never be greater than this value
    public int maxHealthTemporary = 0;
    public int HealthCurrent { get;set; }

    [Export] public int Attack { get;set; }
    public int attackTemporary;
    public int attackPractical { get {return Attack + attackTemporary;} }

    public int Magic { get;set; }
    public int magicTemporary;
    public int magicPractical { get {return Magic + magicTemporary;} }

    public int Defense { get;set; }
    public int defenseTemporary;
    public int defensePractical { get {return Defense + defenseTemporary;} }

    public int Agility { get;set; }
    public int agilityTemporary;
    public int agilityPractical { get {return Agility + agilityTemporary;} }

    StatusEffects currentStatus;
    Dictionary<StatusEffects, float> statusResistances;

    public override void _Ready()
    {
        playerTurnHandler = GetNodeOrNull<PlayerTurnHandler>("./PlayerTurnHandler");

        HealthCurrent = MaxHealthPractical;
        AddToGroup("AliveInBattle");
    }

    public void BasicAttack(Creature target)
    {
        EmitSignal("CreateMessage", $"{CreatureName} attacks {target.CreatureName}");
        target.TakeDamage(Attack);
    }

    public void TakeDamage(int attackStrength)
    {
        //there'll have to be logic for subtracting from temporary hp here as well
        int damageTaken = Mathf.Max(attackStrength - defensePractical, 0); //Make sure you can never take negative damage
        EmitSignal("CreateMessage", $"{CreatureName} takes {damageTaken} points of damage");
        HealthCurrent -= damageTaken; 
        EmitSignal("HealthChanged", HealthCurrent);
    }

    public void Heal(int magicStrength)
    {
        HealthCurrent += magicStrength + rng.RandiRange(0, Mathf.FloorToInt(magicStrength * 0.2f));
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
        GD.Print($"{CreatureName} {HealthCurrent}");
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