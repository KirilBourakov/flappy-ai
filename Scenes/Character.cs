using Godot;
using System;

public partial class Character : CharacterBody2D
{
	private float Speed = 200.0f;
	private float JumpVelocity = -200.0f;
	private int points;

    public override void _Ready()
    {
        this.points = 0;
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
		if (Input.IsActionJustPressed("ui_accept"))
		{
			velocity.Y = JumpVelocity;
		}

		// move character forward
		velocity.X = Speed;


		Velocity = velocity;
		MoveAndSlide();
	}

	public void Kill(){
		this.Speed = 0;
		this.JumpVelocity = 0;
	}

	public void addPoint(){
		this.points++;
	}
}
