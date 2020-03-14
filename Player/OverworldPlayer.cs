using Godot;
using System;
using System.Collections.Generic;

enum PlayerStates
{
    Normal, //regular state, walking around, talking to people, etc
    WarpZoneMovement //player state during the load zone fadeout/fadein
}
public class OverworldPlayer : KinematicBody2D
{
    Node Events;
    Node BetweenMaps;

    const float SPEED = 400;
    PlayerStates currentState = PlayerStates.Normal;
    Vector2 facingDir = new Vector2(0, -1);
    bool doWalkAnimation = false;

    //Normal State Specific
    Vector2 input = new Vector2();

    //WarpZoneMovement State Specific
    const float TRANSITION_FADE_LENGTH = 0.5f;

    public override void _Ready()
    {
        AddToGroup("Player");

        Events = GetNode("/root/Events");
        BetweenMaps = GetNode("/root/BetweenMaps");
    }

    public override void _Process(float delta)
    {
        CalculateNormalizedMoveInput();
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (currentState)
        {
            case PlayerStates.Normal:
                this.MoveAndSlide(input * SPEED);
                if (input != Vector2.Zero)
                {
                    doWalkAnimation = true;
                    facingDir = input.Normalized();
                }
                else 
                {
                    doWalkAnimation = false;
                }

                EntityInteractions();
                Animate();
                break;
            case PlayerStates.WarpZoneMovement:
                MoveAndSlide(facingDir * SPEED);
                Animate();
                break;
        }
    }

    private void CalculateNormalizedMoveInput()
    {
        //input is a class member, so setting it here makes it global to the class
        input.x = (Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"));
	    input.y = (Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up"));
        input = input.Normalized();
    }

    private void EntityInteractions()
    {
        if (Input.IsActionJustPressed("interact")) //idea for interaction with stuff: have an Interact() method that will take the normal of collision as an argument, so it can handle animation
        {
            //cast a ray from the player to look for npcs
            Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
            Godot.Collections.Dictionary intersection = spaceState.IntersectRay(Position, Position + facingDir * SPEED);

            //make sure that the ray both hit something and also that it hit an NPC
            if (intersection.Contains("collider") && (intersection["collider"] as Node).IsInGroup("NPC"))
            {
                StaticNPC npc = intersection["collider"] as StaticNPC;
                npc.Interact((Vector2) intersection["normal"]); //allow the npc to handle the interaction (add an interface to include the Interact() method)
            }
        }
    }

    //This method handles animating the player properly depending on what direction is being faced
    private void Animate()
    {
        AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("./AnimationPlayer");
        if (!doWalkAnimation)
        {
            animationPlayer.Stop(false);
        }
        else
        {
            float angle = Mathf.Rad2Deg(Mathf.Atan2(facingDir.x, -facingDir.y));
            angle = (angle < 0) ? 360 + angle : angle;
            if (angle > 315 || angle < 45)
            {
                animationPlayer.Play("FaceUp");
            }
            else if (angle >= 45 && angle <= 135)
            {
                animationPlayer.Play("FaceRight");
            }
            else if (angle > 135 && angle < 225)
            {
                animationPlayer.Play("FaceDown");
            }
            else if (angle >= 225 && angle <= 315)
            {
                animationPlayer.Play("FaceLeft");
            }
        }
    }

    //sets up the map transition movement and Movement, and then moves to the new map
    public async void MoveToNewMap(Warp entrance, string targetMapScenePath, int exitNumber)
    {
        //setup the player's movement during the fadeout
        currentState = PlayerStates.WarpZoneMovement;
        facingDir = (entrance.Position - Position).Normalized(); //setup the direction the player will walk in during the fade
        doWalkAnimation = true;

        GetNode("/root/BetweenMaps").Call("PrepareForMapChange", exitNumber, facingDir, entrance.outDirectionNormalized);
        Events.EmitSignal("FadeScreen", true, TRANSITION_FADE_LENGTH);
        await ToSignal(GetTree().CreateTimer(TRANSITION_FADE_LENGTH), "timeout");

        GetTree().ChangeScene(targetMapScenePath);
    }

    //Called when the player enters a new map and needs to animate into the map correctly
    public async void OnMapEnter(Warp exit, Vector2 playerMoveDir, Vector2 entranceInDirection)
    {
        //setup the player's movement during the fadein
        Position = exit.Position;
        facingDir = playerMoveDir;
        doWalkAnimation = true;
        currentState = PlayerStates.WarpZoneMovement;

        //handle rotation of the facing direction, if needed
        if (entranceInDirection != exit.outDirectionNormalized)
        {
            facingDir = facingDir.Rotated(entranceInDirection.AngleTo(exit.outDirectionNormalized));
        }
        Events.EmitSignal("FadeScreen", false, TRANSITION_FADE_LENGTH);

        //stop the movement once the player is outside the entrance collision shape
        await ToSignal(exit, "body_exited"); 

        BetweenMaps.Call("MapChangeFinished");
        currentState = PlayerStates.Normal;
    }
}
