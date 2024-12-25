using Godot;
using System;

public partial class CameraMover : Node2D
{
	const int Y = -65;
	public override void _Process(double delta)
	{
		this.GlobalPosition = new Vector2(this.GlobalPosition.X + (float)(200*delta), Y);
	}
}
