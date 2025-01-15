using System.Collections.Generic;
using GdUnit4;
using Godot;
using NEAT;

namespace Test;

[TestSuite]
public class NeuralNetworkTest
{

    [TestCase]
    public void ConstructorTest(){
        GenePool pool = new();
        NeuralNetwork neuralNetwork = new(pool, 1, 2, true);

        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.INPUT).Count).IsEqual(2);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.OUTPUT).Count).IsEqual(2);
    }

    [TestCase]
    public void EvaluateWithDisconnectedBiasTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 2);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        Dictionary<int, NodeGene> map = new(){
            [inp.nodeId] = inp,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output
        };
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes, map);

        double[] neuralIn = new double[] {2};
        double neuralOut = neuralNetwork.Evaluate(neuralIn)[0];

        double expectedOut = neuralIn[0] * inptToHidden.weight * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut).IsEqual(expectedOut);
    }

    [TestCase]
    public void EvaluateWithConnectedBiasTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var bias = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 2);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var biasToHidden = pool.SafeCreateConnectionGene(bias.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        Dictionary<int, NodeGene> map = new(){
            [inp.nodeId] = inp,
            [bias.nodeId] = bias,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output
        };
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes, map);

        double[] neuralIn = new double[] {2};
        double[] neuralOut = neuralNetwork.Evaluate(neuralIn);

        double expectedOut = (neuralIn[0] * inptToHidden.weight + 1 * biasToHidden.weight) * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut[0]).IsEqual(expectedOut);
    }

    [TestCase]
    public void EvaluateWithConnectedBiasUnsortedTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var bias = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 2);

        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var biasToHidden = pool.SafeCreateConnectionGene(bias.nodeId, hidden.nodeId); 
        
        Dictionary<int, NodeGene> map = new(){
            [inp.nodeId] = inp,
            [bias.nodeId] = bias,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output
        };
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes, map);

        double[] neuralIn = new double[] {2};
        double[] neuralOut = neuralNetwork.Evaluate(neuralIn);

        double expectedOut = (neuralIn[0] * inptToHidden.weight + 1 * biasToHidden.weight) * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut[0]).IsEqual(expectedOut);
    }
    
    [TestCase]
    public void EvaluateWithoutBiasTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 2);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        Dictionary<int, NodeGene> map = new(){
            [inp.nodeId] = inp,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output
        };
        NeuralNetwork neuralNetwork = new(pool, false, pool.connectGenes, map);

        double[] neuralIn = new double[] {2};
        double neuralOut = neuralNetwork.Evaluate(neuralIn)[0];

        double expectedOut = neuralIn[0] * inptToHidden.weight * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut).IsEqual(expectedOut);
    }

    [TestCase]
    public void CrossOverTestIsCorrectSize(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var bias = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 2);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var biasToHidden = pool.SafeCreateConnectionGene(bias.nodeId, hidden.nodeId); 

        List <ConnectGene> structure1 = [inptToHidden, HiddenToOut];
        Dictionary<int, NodeGene> map = new(){
            [inp.nodeId] = inp,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output
        };
        NeuralNetwork neuralNetwork1 = new(pool, true, structure1, map);
        neuralNetwork1.fitness = 0;

        List<ConnectGene> structure2 = [biasToHidden,inptToHidden,HiddenToOut];
        map = new(){
            [bias.nodeId] = bias,
            [inp.nodeId] = inp,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output
        };
        NeuralNetwork neuralNetwork2 = new(pool, true, structure2, map);
        neuralNetwork2.fitness = 1;

        NeuralNetwork result = neuralNetwork1.Crossover(neuralNetwork2);

        Assertions.AssertInt(result.structure.Count).IsEqual(3);
    }
}