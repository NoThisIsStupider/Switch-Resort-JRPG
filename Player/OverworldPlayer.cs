using Godot;
using System;

enum PlayerStates 
{
    Normal, //regular state, walking around, talking to people, etc
    LoadZoneMovement //player state during the load zone fadeout/fadein
}
public class OverworldPlayer : KinematicBody2D
{
    PlayerStates currentState = PlayerStates.Normal;
    Vector2 facingDir = new Vector2(0, -1);

    //Normal State Specific
    Vector2 input = new Vector2();

    //LoadZoneMovement State Specific
    Vector2 loadZoneMoveDir = new Vector2();

    public override void _Ready()
    {
        Input.SetUseAccumulatedInput(false);
        GetNode("/root/Events").Connect("LoadZoneEntered", this, "MoveToNewMap");
    }
    
    public override void _PhysicsProcess(float delta)
    {
        switch (currentState)
        {
            case PlayerStates.Normal:
                CalculateNormalizedMoveInput();
                this.MoveAndSlide(input * 100);
                if (input != Vector2.Zero)
                {
                    facingDir = input.Normalized();
                }

                EntityInteractions();
                Animate();
                break;
            case PlayerStates.LoadZoneMovement:
                Position += facingDir; //make a new variable for this, reusing variables for different things is a bit gross
                Animate();
                break;
        }
    }
    
    private void CalculateNormalizedMoveInput()
    {
        //input is a class member
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
    
    private void Animate()
    {
        AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("./AnimationPlayer");
        if (input == Vector2.Zero)
        {
            animationPlayer.Stop(false);
        }
        else
        { 
            if (facingDir.x != 0) //more advanced logic here will be needed once analog inputs are a thing, use trig, 45 degrees should prefer horizontal anims over vertcial
            {
                if (facingDir.x > 0) //since this is a normalized vector, down-right is not quite (1, 1), greater than handles that
                {
                    animationPlayer.Play("FaceRight");
                }
                else
                {
                    animationPlayer.Play("FaceLeft");
                }
            }
            else if (facingDir.y != 0)
            {
                if (facingDir.y > 0) 
                {
                    animationPlayer.Play("FaceDown");
                }
                else
                {
                    animationPlayer.Play("FaceUp");
                }
            }
        }
    }
    
    //sets up the map transition movement and Movement, and then moves to the new map
    public async void MoveToNewMap(string targetMapScenePath, int targetEntrance, Vector2 targetMovePosition) //targetMovePosition refers to where the player should move to during the fadeout
    {
        currentState = PlayerStates.LoadZoneMovement;
        facingDir = targetMovePosition - Position;
        facingDir = facingDir.Normalized();

        GetNode("/root/Events").EmitSignal("FadeScreen", true);

        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        GetTree().ChangeScene(targetMapScenePath);
    }

    //Called when the player enters a new map and needs to animate into the map correctly
    public void OnMapEnter(Vector2 EntrancePosition)
    {

    }
}
