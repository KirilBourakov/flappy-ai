using Godot;
using System;

public partial class ModelManager : Node2D
{
	
	PackedScene modelScene;
	ModelState modelState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.modelScene = GD.Load<PackedScene>("res://Scenes/Agents/Model/ModelManager.tscn");
		this.modelState = GD.Load<ModelState>("res://Scenes/Agents/Model/ModelState.cs");
		
		if (!this.modelState.internalManaged)
		{
			for (int i = 0; i < ModelState.GEN_SIZE; i++)
			{
				var newModel = (ModelPlayer)this.modelScene.Instantiate();
				this.modelState.AddModel(newModel);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (AllDead()){
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
