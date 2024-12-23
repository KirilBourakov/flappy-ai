using Godot;
using System;

public partial class ModelManager : Node2D
{
	public struct ModelTracker
    {
        public ModelPlayer model;
        public int fitness;

		public ModelTracker(ModelPlayer model){
			this.model = model;
			this.fitness = 0;
		}
    }

	private ModelTracker[] models;
	private const int GEN_SIZE = 1000;


	PackedScene modelScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.modelScene = GD.Load<PackedScene>("res://Scenes/Agents/Model/ModelManager.tscn");

		this.models = new ModelTracker[GEN_SIZE];
		for (int i = 0; i < GEN_SIZE; i++)
		{
			this.models[i] = new ModelTracker(
				(ModelPlayer)this.modelScene.Instantiate()
			);
			this.AddChild(this.models[i].model);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
