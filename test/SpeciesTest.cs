using System;
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
        List<NeuralNetwork> members = [
            new NeuralNetwork(pool, true, [], []),
            new NeuralNetwork(pool, true, [], []),
            new NeuralNetwork(pool, true, [], [])
        ];
        members[0].fitness = 2;
        members[0].adjustedFitness = 2/3d;
        members[1].fitness = 3;
        members[1].adjustedFitness = 1;
        members[2].fitness = 4;
        members[2].adjustedFitness = 4/3d;
        Species species = new(members);
        return species;
    }

    [TestCase]
    public void updateAvgFitnessTest(){
        Species species = genSpecies();
        double avg = species.updateFitnessFields();

        Assertions.AssertThat(avg).IsEqual(1d);
        Assertions.AssertThat(species.avgAdjustedFitness).IsEqual(1d);
        Assertions.AssertThat(species.totalFitness).IsEqual(9);
    }

    [TestCase]
    public void CreateNewGenerationCorrectLengthTest(){
        Species species = genSpecies();
        species.updateFitnessFields();

        //Assertions.AssertThat(species.avgAdjustedFitness).IsEqual(1100);
        Assertions.AssertInt(species.CreateNewGeneration(species.avgAdjustedFitness, out _).Count).IsEqual(species.memebers.Count);
        Assertions.AssertInt(species.CreateNewGeneration(species.avgAdjustedFitness*2, out _).Count).IsEqual((int) Math.Round(species.memebers.Count/2d));
        Assertions.AssertInt(species.CreateNewGeneration(species.avgAdjustedFitness/2, out _).Count).IsEqual(species.memebers.Count*2);
    }
}