using Godot;
using System;

public partial class ModelState : Node
{
	public static ModelState Instance { get; private set; }
	public const int GEN_SIZE = 20;

	public ModelPlayer[] models {get;set;}
	private int modelsCount;
	public bool internalManaged = false;
	private Random random = new Random();

	public int generationNumber = 1;
	public float furthestDistanceTraveled = 0;
	public float currentGenerationDistancedTraveled = 0;

	public override void _Ready()
    {
        Instance = this;
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
		this.generationNumber++;
		this.furthestDistanceTraveled = Math.Max(this.furthestDistanceTraveled, this.currentGenerationDistancedTraveled);
		this.currentGenerationDistancedTraveled = 0;

		int totalFitness = 0;
		this.internalManaged = true;

		// generate the reproduction wheel
		foreach (var model in this.models)
		{
			totalFitness += model.GetFitness();
		}

		ModelPlayer[] wheel = new ModelPlayer[totalFitness];
		int index = 0;

		foreach (var model in this.models)
		{
			for (int j = 0; j < model.GetFitness(); j++)
			{
				wheel[index] = model;
				index++;
			}
		}

		//reproduce
		ModelPlayer[] newGeneration = new ModelPlayer[GEN_SIZE];

		// elitism; keep top 5% of models (min 1) that have more then 1 point
		int elitismSize = (int)Math.Ceiling(0.05 * GEN_SIZE);
		Array.Sort(models, (x, y) => y.GetFitness().CompareTo(x.GetFitness()));

		int i = 0;
		while(i < elitismSize){
			if (models[i].GetFitness() > 1){
				newGeneration[i] = models[i];
				i++;
			} 
			else {
				break;
			}
		}

		while(i < GEN_SIZE){
			// get the two parents
			ModelPlayer mate1 = wheel[random.Next(0, totalFitness)];
			ModelPlayer mate2 = wheel[random.Next(0, totalFitness)];
			while (mate1 == mate2){
				mate2 = wheel[random.Next(0, totalFitness)];
			}
			GD.Print(mate1, mate2);
			ModelPlayer child = new ModelPlayer();
			for (int j = 0; j < child.weights.Length; j++){
				// choose a random parent gene
				child.weights[j] = random.Next(0, 2) == 1 ? mate2.weights[j] : mate1.weights[j];
				//apply a mutation between +1 and -1
				child.weights[j] += GetGaussianMutation();
			}
			newGeneration[i] = child;
			i++;
		}
		this.models = newGeneration;
	}

	public double GetGaussianMutation(){
		double u1 = random.NextDouble();
		double u2 = random.NextDouble();
		return 0.1 * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
	}



}
