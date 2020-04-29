using Godot;
using System;
using System.Collections.Generic;

//The logic for the actual turns would use an async function that waits for the creature to handle its turn before moving to the next one

class BattleController : Node2D
{
    List<Creature> turnOrder = new List<Creature>();
    int currentTurn = 0;
    bool waitingForTurn = false;
    public override void _Ready()
    {
        //add some test creatures here while developing
        Creature player = (Creature) (GD.Load("res://Scenes/Battle/BattlePlayer.tscn") as PackedScene).Instance();
        AddToBattle(player);

        Creature enemy = (Creature) (GD.Load("res://Scenes/Battle/BattleEnemy.tscn") as PackedScene).Instance();
        AddToBattle(enemy);
    }

    //may not need process, might be able to do the battle system as a series of signals and callbacks
    public override void _Process(float delta)
    {
        if (waitingForTurn)
        {

        }
        else 
        {
            HandleTurn(turnOrder[currentTurn]);
        }
    }

    private async void HandleTurn(Creature creature)
    {
        waitingForTurn = true;
        creature.DoTurn();
        if (creature.CreatureName == "Player")
        {
            await ToSignal(creature, "TurnFinished");
        }
        currentTurn = (currentTurn + 1) % turnOrder.Count;
        waitingForTurn = false;
    }

    private void StartBattle()
    {

    }

    private void EndBattle()
    {
        
    }

    private void GenerateTurnOrder()
    {
        foreach (Creature creature in GetTree().GetNodesInGroup("AliveInBattle"))
        {
            turnOrder.Add(creature);
        }
        turnOrder.Sort(CompareCreatureAgility);
    }

    private int CompareCreatureAgility(Creature a, Creature b)
    {
        if (a.Agility == b.Agility)
        {
            return 0;
        }
        else if (a.Agility > b.Agility)
        {
            return 1;
        }
        else 
        {
            return -1;
        }
    }

    private void AddToBattle(Creature creature) //someone being revived or new enemies appearing
    {
        //Make sure to run request ready on the creature, in case ready has already been run for that instance
        turnOrder.Add(creature);
        AddChild(creature);
    }

    private void RemoveFromBattle(Creature creature) //basically, a creature dying
    {

    }
}