using Godot;
using System;
using NEAT;
using System.Collections.Generic;

public partial class ModelManager : Node2D
{
	
	PackedScene modelScene;
	ModelState modelState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.modelScene = GD.Load<PackedScene>("res://Scenes/Agents/Model/ModelPlayer.tscn");

		this.modelState = ModelState.Instance;
		if (!this.modelState.internalManaged)
		{
			for (int i = 0; i < ModelState.INITAL_SIZE; i++)
			{
				var newModel = (ModelPlayer) modelScene.Instantiate();
				newModel.Position = new Vector2(0, -65);
				newModel.neuralNetwork = new(modelState.pool, ModelPlayer.INPUT_NUM, 1, true);

				ModelState.Instance.models.Add(newModel);
				AddChild(newModel);
			}
		} else {
			List<ModelPlayer> newModels = new();
			for (int i = 0; i < ModelState.Instance.models.Count; i++){
				var newModel = (ModelPlayer) modelScene.Instantiate();
				newModel.Position = new Vector2(0, -65);
				newModel.neuralNetwork = ModelState.Instance.models[i].neuralNetwork;

				newModels.Add(newModel);
				AddChild(newModel);
			}
			ModelState.Instance.models = newModels;
		}
		GD.Print(modelState.pool.connectGenes.Count);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool finished = AllDead();
		if (finished && !this.modelState.reproductionLocked){
			this.modelState.Reproduce();
			// reload scene
			GetTree().ReloadCurrentScene();
		}
	}

	public bool AllDead(){
		if (this.modelState.models.Count == 0){
			throw new Exception("No models within modelState");
		}
		foreach (var model in this.modelState.models)
		{
			if (!model.dead){
				return false;
			}
		}
		return true;
	}
}
