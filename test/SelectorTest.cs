using System.Collections.Generic;
using GdUnit4;
using Godot;
using NEAT;

namespace Test;

[TestSuite]
public class SelectorTest 
{
    [TestCase]
    public void CompareSameNetworkTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 2);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        Dictionary<int, NodeGene> map = new Dictionary<int, NodeGene>(){
            [inp.nodeId] = inp,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output,
        };
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes, map);

        Population selector = new(1, 1, 1, false, out _);

        Assertions.AssertThat(selector.Compare(neuralNetwork, neuralNetwork)).IsEqual(0d);

    }

    [TestCase]
    public void CompareSimilarNetworkTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 2);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var inptToOut = pool.SafeCreateConnectionGene(inp.nodeId, output.nodeId);

        Dictionary<int, NodeGene> map = new Dictionary<int, NodeGene>(){
            [inp.nodeId] = inp,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output,
        };
        NeuralNetwork large = new(pool, true, pool.connectGenes, map);

        map = new()
        {
            [inp.nodeId] = inp,
            [hidden.nodeId] = hidden,
            [output.nodeId] = output
        };

        NeuralNetwork small = new (pool, true, [inptToHidden, HiddenToOut, inptToOut], map);

        Population selector = new(1, 1, 1, false, out _);

        Assertions.AssertThat(selector.Compare(large, small)).IsLess(selector.threshold);
    }

    [TestCase]
    public void CompareDifferentNetworkTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var inp2 = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var hidden2 = pool.CreateNode(NodeGene.Type.HIDDEN, 1);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT, 1);


        var inpToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var inpToHidden2 = pool.SafeCreateConnectionGene(inp.nodeId, hidden2.nodeId);

        var inp2ToHidden = pool.SafeCreateConnectionGene(inp2.nodeId, hidden.nodeId);
        var inp2ToHidden2 = pool.SafeCreateConnectionGene(inp2.nodeId, hidden2.nodeId);

        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var Hidden2ToOut = pool.SafeCreateConnectionGene(hidden2.nodeId, output.nodeId);
        var inpToOut = pool.SafeCreateConnectionGene(inp.nodeId, output.nodeId);


        Dictionary<int, NodeGene> map = new()
        {
            [inp.nodeId] = inp,
            [output.nodeId] = output
        };
        NeuralNetwork small = new(pool, true, [inpToOut], map);
        map = new()
        {
            [inp.nodeId] = inp,
            [inp2.nodeId] = inp2,
            [hidden.nodeId] = hidden,
            [hidden2.nodeId] = hidden2,
            [output.nodeId] = output
        };
        NeuralNetwork large = new(pool, true, [inpToHidden, inp2ToHidden2, inp2ToHidden, inpToHidden2, HiddenToOut, Hidden2ToOut], map);

        Population selector = new(1, 1, 1, false, out _);
        Assertions.AssertThat(selector.Compare(small, large)).IsGreater(selector.threshold);
    }

}