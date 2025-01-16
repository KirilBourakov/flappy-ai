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
        members[1].fitness = 3;
        members[2].fitness = 4;
        Species species = new(members);
        return species;
    }
}