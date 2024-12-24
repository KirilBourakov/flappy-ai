using Godot;
using System;

public partial class Train : Node2D
{
	private Camera2D camera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = new Camera2D();
		camera.Zoom = new Vector2(2.5f, 2.5f);
		camera.Enabled = true;
		AddChild(camera);
        camera.MakeCurrent();

        camera.GlobalPosition = new Vector2(0, -65);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		camera.GlobalPosition = new Vector2(camera.GlobalPosition.X + (float)(200*delta), camera.GlobalPosition.Y);
	}
}
