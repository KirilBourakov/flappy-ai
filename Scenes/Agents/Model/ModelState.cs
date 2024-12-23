using Godot;
using System;

public partial class ModelState : Node
{
	public const int GEN_SIZE = 1000;

	public ModelPlayer[] models {get;set;}
	private int modelsCount;
	public bool internalManaged = false;

	public ModelState(){
		this.models = new ModelPlayer[GEN_SIZE];
		this.modelsCount = 0;
	}

	public bool AddModel(ModelPlayer model){
		bool success = false;
		if (modelsCount < this.models.Length){
			this.models[modelsCount] = model;
			modelsCount++;
			success = true;
		}
		return success;
	}

	public void Reproduce(){
		int totalFitness = 0;
		Random random = new Random();

		foreach (var model in this.models)
		{
			totalFitness += model.GetPoints()+1;
		}

		ModelPlayer[] wheel = new ModelPlayer[totalFitness];

		int index = 0;
		foreach (var model in this.models)
		{
			for (int i = 0; i < model.GetPoints(); i++)
			{
				wheel[index] = model;
				index++;
			}
		}

		ModelPlayer[] newGeneration = new ModelPlayer[GEN_SIZE];
		for (int i = 0; i < GEN_SIZE; i++){
			// get the two parents
			ModelPlayer mate1 = this.models[i];
			ModelPlayer mate2 = wheel[random.Next(0, totalFitness)];

			ModelPlayer child = new ModelPlayer();
			for (int j = 0; j <= child.weights.Length; j++){
				// choose a random parent gene
				child.weights[j] = random.Next(0, 2) == 1 ? mate2.weights[j] : mate1.weights[j];

				//apply a mutation between +1 and -1
				child.weights[j] += (random.NextDouble()*2)-1;
			}
			newGeneration[i] = child;
		}
		this.models = newGeneration;
	}



}
