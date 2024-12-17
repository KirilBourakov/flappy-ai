using Godot;
using System;

public partial class Character : CharacterBody2D
{
	private float Speed = 200.0f;
	private float JumpVelocity = -200.0f;
	private int points;
	private Label label;
	private AudioStreamPlayer2D scorePlayer;

    public override void _Ready()
    {
        this.points = 0;
		this.label = GetNode<Label>("Camera2D/CoinLabel");
		this.scorePlayer = GetNode<AudioStreamPlayer2D>("ScorePlayer");
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
		this.label.Text = "Score " + this.points;
		this.scorePlayer.Play();
	}
}
