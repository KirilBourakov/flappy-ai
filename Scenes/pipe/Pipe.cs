using Godot;
using System;

public partial class Pipe : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private Node2D parent;
	public override void _Ready()
	{
		parent = GetParent<Node2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ScreenExit(){
		parent.QueueFree();
	}
}
