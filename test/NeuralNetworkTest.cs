using System.Collections.Generic;
using GdUnit4;
using Godot;
using NEAT;

[TestSuite]
public class NeuralNetworkTest
{

    [TestCase]
    public void ConstructorTest(){
        GenePool pool = new();
        NeuralNetwork neuralNetwork = new(pool, 1, 2, true);

        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.INPUT).Count).Equals(1);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.OUTPUT).Count).Equals(2);
    }

    [TestCase]
    public void EvaluateWithDisconnectedBiasTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes);

        double[] neuralIn = new double[] {2};
        double neuralOut = neuralNetwork.Evaluate(neuralIn)[0];

        double expectedOut = neuralIn[0] * inptToHidden.weight * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut).Equals(expectedOut);
    }

    [TestCase]
    public void EvaluateWithConnectedBiasTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var bias = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var biasToHidden = pool.SafeCreateConnectionGene(bias.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes);

        double[] neuralIn = new double[] {2};
        double[] neuralOut = neuralNetwork.Evaluate(neuralIn);

        double expectedOut = (neuralIn[0] * inptToHidden.weight + 1 * biasToHidden.weight) * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut).Equals(expectedOut);
    }

    [TestCase]
    public void EvaluateWithConnectedBiasUnsortedTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var bias = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);

        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var biasToHidden = pool.SafeCreateConnectionGene(bias.nodeId, hidden.nodeId); 
        
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes);

        double[] neuralIn = new double[] {2};
        double[] neuralOut = neuralNetwork.Evaluate(neuralIn);

        double expectedOut = (neuralIn[0] * inptToHidden.weight + 1 * biasToHidden.weight) * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut).Equals(expectedOut);
    }
    
    [TestCase]
    public void EvaluateWithoutBiasTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        NeuralNetwork neuralNetwork = new(pool, false, pool.connectGenes);

        double[] neuralIn = new double[] {2};
        double neuralOut = neuralNetwork.Evaluate(neuralIn)[0];

        double expectedOut = neuralIn[0] * inptToHidden.weight * HiddenToOut.weight;

        Assertions.AssertThat(neuralOut).Equals(expectedOut);
    }

    [TestCase]
    public void CrossOverTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var bias = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var biasToHidden = pool.SafeCreateConnectionGene(bias.nodeId, hidden.nodeId); 

        List <ConnectGene> structure1 = [inptToHidden, HiddenToOut];
        NeuralNetwork neuralNetwork1 = new(pool, true, structure1);
        neuralNetwork1.fitness = 0;

        List<ConnectGene> structure2 = [biasToHidden,inptToHidden,HiddenToOut];
        NeuralNetwork neuralNetwork2 = new(pool, true, structure2);
        neuralNetwork2.fitness = 1;

        NeuralNetwork result = neuralNetwork1.Crossover(neuralNetwork2);

        Assertions.AssertInt(result.structure.Count).Equals(3);
    }
}