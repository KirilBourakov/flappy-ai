using Godot;
using System;

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
			for (int i = 0; i < ModelState.GEN_SIZE; i++)
			{
				var newModel = (ModelPlayer)this.modelScene.Instantiate();
				newModel.Position = new Vector2(0, -65);
				this.modelState.AddModel(newModel);
				AddChild(newModel);
			}
		} else {
			for (int i = 0; i < this.modelState.models.Length; i++)
			{
				var newModel = (ModelPlayer)this.modelScene.Instantiate();
				newModel.Position = new Vector2(0, -65);
				newModel.neuralNetwork = this.modelState.models[i].neuralNetwork;

				this.modelState.models[i] = newModel;
				AddChild(newModel);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool finished = AllDead();
		if (finished){
			this.modelState.Reproduce();
			// reload scene
			GetTree().ReloadCurrentScene();
		}
	}

	public bool AllDead(){
		foreach (var model in this.modelState.models)
		{
			if (!model.dead){
				return false;
			}
		}
		return true;
	}
}
