using Godot;
using System;

public partial class ModelPlayer : Agent
{
	private bool dead = false;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		//TODO!!

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
