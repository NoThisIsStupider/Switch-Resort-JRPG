using Godot;
using System;

public class OverworldPlayer : KinematicBody2D
{
    Vector2 facingDir = new Vector2(0, -1);
    Vector2 input = new Vector2();
    
    public override void _Ready()
    {
        Input.SetUseAccumulatedInput(false);
    }

    public override void _PhysicsProcess(float delta)
    {
        input.x = Convert.ToInt32(Input.IsActionPressed("ui_right")) - Convert.ToInt32(Input.IsActionPressed("ui_left"));
        input.y = Convert.ToInt32(Input.IsActionPressed("ui_down")) - Convert.ToInt32(Input.IsActionPressed("ui_up"));
        this.MoveAndSlide(input.Normalized() * 100);

        if (input != Vector2.Zero)
        {
            facingDir = input.Normalized();
        }

        //this will be removed once there are proper player sprites to indicate the same information, all it is is an indicator for where the arrow is facing
        Polygon2D facingDirIndicator = GetNode<Polygon2D>("./FacingDirIndicator");
        facingDirIndicator.Position = facingDir * 8;
        facingDirIndicator.Rotation = Mathf.Atan2(facingDir.x, -facingDir.y);

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
        Animate();
    }

    public void Animate()
    {
        AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("./AnimationPlayer");
        if (input == Vector2.Zero)
        {
            animationPlayer.Stop(false);
        }
        else
        {
            if (facingDir.x != 0)
            {
                if (facingDir.x > 0)
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
                if (facingDir.y > 0) //since this is a normalized vector, down-right is not quite (1, 1), greater than handles that
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
}
