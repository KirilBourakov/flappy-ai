using System.Collections.Generic;
using GdUnit4;
using Godot;
using NEAT;

namespace Test;

[TestSuite]
public class SpeciesTest 
{
    public Species genSpecies(){
        GenePool pool = new();
        List<NeuralNetwork> members = new List<NeuralNetwork>{
            new NeuralNetwork(pool, true, new List<ConnectGene>()),
            new NeuralNetwork(pool, true, new List<ConnectGene>()),
            new NeuralNetwork(pool, true, new List<ConnectGene>())
        };
        members[0].fitness = 2;
        members[1].fitness = 3;
        members[2].fitness = 4;
        Species species = new(members);
        return species;
    }

    [TestCase]
    public void updateAvgFitnessTest(){
        Species species = genSpecies();
        double avg = species.updateFitnessFields();

        Assertions.AssertThat(avg).Equals(3d);
        Assertions.AssertThat(species.avgAdjustedFitness).Equals(3d);
    }

    [TestCase]
    public void CreateNewGenerationCorrectLengthTest(){
        Species species = genSpecies();
        species.updateFitnessFields();

        Assertions.AssertInt(species.CreateNewGeneration(species.avgAdjustedFitness).Count).IsEqual(species.memebers.Count);
        Assertions.AssertInt(species.CreateNewGeneration(species.avgAdjustedFitness*2).Count).IsEqual(species.memebers.Count/2);
        Assertions.AssertInt(species.CreateNewGeneration(species.avgAdjustedFitness/2).Count).IsEqual(species.memebers.Count*2);
    }
}