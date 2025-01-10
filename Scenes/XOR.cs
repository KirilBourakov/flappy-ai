using System.Collections.Generic;
using System.Threading;
using Godot;
using NEAT;


public partial class XOR : Node2D
{
	private Thread thread;
	private static double[][] xorInputs = new double[][]{
        [0.0, 0.0],
        [0.0, 1.0],
        [1.0, 0.0],
        [1.0, 1.0]
    };
    private static double[] xorOutputs = new double[]{0.0,1.0,1.0, 0.0};
	private const int GEN_SIZE = 50;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		thread = new(Run);
		thread.Start();
	}
    
    public void Run(){
        List<NeuralNetwork> population = new();

        GenePool pool = new();
        for (int i = 0; i < GEN_SIZE; i++){
            population.Add(new NeuralNetwork(pool, 2, 1, false));
        }

        Selector selector = new();

        int gen = 1;
        while (true){
            foreach (var network in population)
            {
                test(network);
            }

            GD.Print($"Generation {gen} has a pool of {pool.connectGenes.Count} genes and {population.Count} members");
            double avg = 0;
            foreach (var network in population)
            {
                avg += network.fitness;
            }
            avg /= population.Count;
			population.Sort((x, y) => y.fitness.CompareTo(x.fitness));
            GD.Print($"Average fitness is {avg} while highest is {population[0].fitness}");

            population = selector.CreateNewGeneration(population);
            gen++;
        }
    }

    public static void test(NeuralNetwork network){
        network.fitness = 0;
        for (int i = 0; i < xorInputs.Length; i++){
            double result = network.Evaluate(xorInputs[i])[0];

            if (result > 0.5 && xorOutputs[i] == 1){
                network.fitness++;
            }
            else if (result < 0.5 && xorOutputs[i] == 0){
                network.fitness++;
            }
        }
    }
}
