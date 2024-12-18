using Godot;
using System;

public partial class Camera2d : Camera2D
{

	private float Y;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Y = Position.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.GlobalPosition = new Vector2(this.GlobalPosition.X, this.Y);
	}
}
