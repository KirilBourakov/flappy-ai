using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ModelPlayer : Agent
{
	private bool dead = false;

	private double[] weights;

	private RayCast2D[] inputs = new RayCast2D[9];

    // distance from ground
    // 'sight'

    public override void _Ready()
    {
    	var children = GetNode<Node2D>("Raycasts").GetChildren();
		int i = 0;
		foreach (RayCast2D child in children.Cast<RayCast2D>())
		{
			if (i < 9){
				inputs[i] = child;
				i++;
			} else {
				break;
			}
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		foreach (var input in this.inputs){
			if (input.IsColliding()){
				GD.Print(input.Name + " is collding");
			}
		}
		if (Input.IsActionJustPressed("ui_accept"))
		{
			velocity.Y = JumpVelocity;
		}

		velocity.X = Speed;

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void Kill()
    {
        this.dead = true;
    }
    public override void AddPoint()
    {
        this.points++;
    }
}
