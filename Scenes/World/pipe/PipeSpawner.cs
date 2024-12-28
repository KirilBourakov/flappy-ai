using Godot;
using System;

public partial class PipeSpawner : Node2D
{
	private Timer timer;
	private PackedScene pipeScene;
	private PackedScene pointScene;
	private Random random;

	private const int CENTER_MIN = -120;
	private const int CENTER_MAX = -40;
	private const int GAP = 50;

	// Called when the node enters the scene tree for the first time.

	private int center;
	private Vector2 SpawnPos;
	public override void _Ready()
	{
		this.pipeScene = GD.Load<PackedScene>("res://Scenes/World/pipe/Pipe.tscn");
		this.pointScene = GD.Load<PackedScene>("res://Scenes/World/pipe/PointGiver.tscn");
		this.timer = GetNode<Timer>("Timer");
		this.timer.Start();

		this.SpawnPos = new Vector2(0,0);
		this.random = new Random();

		this.center = this.random.Next(CENTER_MIN, CENTER_MAX);
	}

	public void SpawnPipes(){
		float x = this.SpawnPos.X+100;

		int newCenter = this.center + this.random.Next(-40, 40);
		while (newCenter > CENTER_MAX || newCenter < CENTER_MIN){
			GD.Print("looking for new center");
			newCenter = this.center + this.random.Next(-40, 40);
		}
		this.center = newCenter;
		
		Pipe top = (Pipe)this.pipeScene.Instantiate();
		top.Position = new Vector2(x, center-(GAP/2)-75);
		top.RotationDegrees += 180;

		Pipe bottom = (Pipe)this.pipeScene.Instantiate();
		bottom.Position = new Vector2(x, center+(GAP/2)+75);

		PointGiver pointGiver = (PointGiver)this.pointScene.Instantiate();
		pointGiver.Position = new Vector2(x, 0);
		CollisionShape2D giverColliderNode = pointGiver.GetNode<CollisionShape2D>("CollisionShape2D");

		// Check if the shape is a RectangleShape2D
		if (giverColliderNode.Shape is RectangleShape2D rectangleShape)
		{
			// Set the extents of the RectangleShape2D
			rectangleShape.Size = new Vector2(30, 400);
		}
		this.AddChild(pointGiver);
		this.AddChild(bottom);
		this.AddChild(top);
	}

	public void SpawnTimerDone(){
		SpawnPipes();
		this.timer.Start();
	}

    public override void _PhysicsProcess(double delta)
    {
        this.SpawnPos = new Vector2(this.SpawnPos.X+(float)(200.0f*delta),SpawnPos.Y);
    }
}
