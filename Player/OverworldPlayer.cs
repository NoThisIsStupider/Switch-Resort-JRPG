using Godot;
using System;

enum PlayerStates
{
    Normal, //regular state, walking around, talking to people, etc
    WarpZoneMovement //player state during the load zone fadeout/fadein
}
public class OverworldPlayer : KinematicBody2D
{
    Node Events;
    Node BetweenMaps;

    const float SPEED = 100;
    PlayerStates currentState = PlayerStates.Normal;
    Vector2 facingDir = new Vector2(0, -1);
    bool doWalkAnimation = false;

    //Normal State Specific
    Vector2 input = new Vector2();

    //WarpZoneMovement State Specific
    const float TRANSITION_FADE_LENGTH = 0.5f;

    public override void _Ready()
    {
        Events = GetNode("/root/Events");
        BetweenMaps = GetNode("/root/BetweenMaps");

        Events.Connect("WarpEntered", this, "MoveToNewMap");
        Events.Connect("NewMapEntered", this, "OnMapEnter");
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (currentState)
        {
            case PlayerStates.Normal:
                CalculateNormalizedMoveInput();
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
        input.x = Convert.ToInt32(Input.IsActionPressed("ui_right")) - Convert.ToInt32(Input.IsActionPressed("ui_left"));
        input.y = Convert.ToInt32(Input.IsActionPressed("ui_down")) - Convert.ToInt32(Input.IsActionPressed("ui_up"));
        input = input.Normalized();
    }

    private void EntityInteractions()
    {
        if (Input.IsActionJustPressed("ui_accept")) //idea for interaction with stuff: have an Interact() method that will take the normal of collision as an argument, so it can handle animation
        {
            Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
            Godot.Collections.Dictionary intersection = spaceState.IntersectRay(Position, Position + facingDir * 10);
            if (intersection.Contains("collider"))
            {
                Node thing = (Node)intersection["collider"];
                GetNode("/root/Events").EmitSignal("ShowMessage", $"{thing.Name}");
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
    //this is called by the WarpEntered signal defined in the Events singleton, and that signal is emitted by Warps
    public async void MoveToNewMap(Node2D entrance, string targetMapScenePath, int exitNumber)
    {
        //setup the player's movement during the fadeout
        currentState = PlayerStates.WarpZoneMovement;
        facingDir = (entrance.Position - Position).Normalized(); //setup the direction the player will walk in during the fade
        doWalkAnimation = true;

        GetNode("/root/BetweenMaps").Call("PrepareForMapChange", exitNumber, facingDir);

        Events.EmitSignal("FadeScreen", true, TRANSITION_FADE_LENGTH);
        await ToSignal(GetTree().CreateTimer(TRANSITION_FADE_LENGTH), "timeout");

        GetTree().ChangeScene(targetMapScenePath);
    }

    //Called when the player enters a new map and needs to animate into the map correctly
    //this is called by the NewMapEntered signal defined in the Events singleton, and that signal is emitted by the BetweenMaps singleton
    public async void OnMapEnter(Node2D exit, Vector2 playerMoveDir)
    {
        //setup the player's movement during the fadein
        Position = exit.Position;
        facingDir = playerMoveDir;
        doWalkAnimation = true;
        currentState = PlayerStates.WarpZoneMovement;

        Events.EmitSignal("FadeScreen", false, TRANSITION_FADE_LENGTH);

        //stop the movement once the player is outside the entrance collision shape
        await ToSignal(exit, "body_exited"); 

        BetweenMaps.Call("MapChangeFinished");
        currentState = PlayerStates.Normal;
    }
}
