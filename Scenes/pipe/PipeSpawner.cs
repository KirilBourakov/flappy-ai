using Godot;
using System;

public partial class PipeSpawner : Node2D
{
	private Timer timer;
	private PackedScene pipeScene;
	private PackedScene pointScene;
	private Random random;

	private const int CENTER_MIN = -150;
	private const int CENTER_MAX = -20;
	private const int LAST_GAP_MIN = 50;
	private const int LAST_GAP_MAX = 100;

	// Called when the node enters the scene tree for the first time.

	private int lastCenter;
	private int lastGap;
	private Vector2 SpawnPos;
	public override void _Ready()
	{
		this.pipeScene = GD.Load<PackedScene>("res://Scenes/pipe/Pipe.tscn");
		this.pointScene = GD.Load<PackedScene>("res://Scenes/pipe/PointGiver.tscn");
		this.timer = GetNode<Timer>("Timer");
		this.timer.Start();

		this.SpawnPos = new Vector2(0,0);
		this.random = new Random();

		this.lastCenter = this.random.Next(CENTER_MIN, CENTER_MAX);
		this.lastGap = this.random.Next(LAST_GAP_MIN, LAST_GAP_MAX);
	}

	public void SpawnPipes(){
		float x = this.SpawnPos.X+100;
		int center = Math.Max(CENTER_MIN, this.lastCenter + this.random.Next(-25, +25));
		center = Math.Min(CENTER_MAX, center);
		int gap = Math.Max(LAST_GAP_MIN, this.lastGap + this.random.Next(-25, +25));
		gap = Math.Min(LAST_GAP_MAX, gap);
		
		Pipe top = (Pipe)this.pipeScene.Instantiate();
		top.Position = new Vector2(x, center-(gap/2)-75);
		top.RotationDegrees += 180;

		Pipe bottom = (Pipe)this.pipeScene.Instantiate();
		bottom.Position = new Vector2(x, center+(gap/2)+75);

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
		GD.Print("Spawn " + SpawnPos.X);
		SpawnPipes();
		timer.WaitTime = this.random.NextDouble()+0.25;
		this.timer.Start();
	}

    public override void _PhysicsProcess(double delta)
    {
        this.SpawnPos = new Vector2(this.SpawnPos.X+(float)(200.0f*delta),SpawnPos.Y);
    }
}
