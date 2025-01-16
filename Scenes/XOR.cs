using System;
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
        Population population = new(GEN_SIZE, 2, 1, false, out GenePool pool);

        int gen = 1;
        while (true){
            foreach (var species in population.population)
            {
                foreach (var member in species.memebers){
                    test(member);
                }
            }


            GD.Print($"Generation {gen} has a pool of {pool.connectGenes.Count} genes and {population.totalMembers} members");

            double avg = 0;
            foreach (var species in population.population)
            {
                foreach (var member in species.memebers){
                    avg += member.fitness;
                }
                
            }

            avg /= population.population.Count;
            GD.Print($"Average fitness is {avg / xorInputs.Length}");

            GD.Print("creating new generation");
            population.CreateNewGeneration();
            GD.Print("created new generation");
            gen++;
        }
    }

    public static void test(NeuralNetwork network){
        network.fitness = 0;
        for (int i = 0; i < xorInputs.Length; i++){
            double result = network.Evaluate(xorInputs[i])[0];

            network.fitness += 1 - Math.Abs(result - xorOutputs[i]);
            // if (result > 0.5 && xorOutputs[i] == 1){
            //     network.fitness++;
            // }
            // else if (result < 0.5 && xorOutputs[i] == 0){
            //     network.fitness++;
            // }
        }
    }
}
