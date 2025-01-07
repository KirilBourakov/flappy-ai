using Godot;
using System;
using NEAT;
using System.Collections.Generic;

public partial class ModelState : Node
{
	public static ModelState Instance { get; private set; }
	public const int INITAL_SIZE = 50;

	public List<ModelPlayer> models =  new(INITAL_SIZE);
	public bool internalManaged = false;
	private Selector selector = new Selector();

	public int generationNumber = 1;
	public float furthestDistanceTraveled = 0;
	public float currentGenerationDistancedTraveled = 0;

	public override void _Ready()
    {
        Instance = this;
    }

	public void Reproduce(){
		generationNumber += 1;
		internalManaged = true;
		furthestDistanceTraveled = Math.Max(furthestDistanceTraveled, currentGenerationDistancedTraveled);
		currentGenerationDistancedTraveled = 0;

		List<NeuralNetwork> gen = new List<NeuralNetwork>();
		for (int i = 0; i < this.models.Count; i++){
			gen.Add(this.models[i].neuralNetwork);
		}
		gen = selector.CreateNewGeneration(gen);
		List<ModelPlayer> temp = new();
		for (int i = 0; i< gen.Count; i++){
			temp.Add(new ModelPlayer(gen[i]));
		}

		this.models = temp;
	}

}
