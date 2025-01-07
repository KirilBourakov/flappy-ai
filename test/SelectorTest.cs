using GdUnit4;
using NEAT;


[TestSuite]
public class SelectorTest 
{
    [TestCase]
    public void CompareSameNetworkTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        
        NeuralNetwork neuralNetwork = new(pool, true, pool.connectGenes);

        Selector selector = new();

        Assertions.AssertThat(selector.Compare(neuralNetwork, neuralNetwork)).Equals(0d);

    }

    [TestCase]
    public void CompareSimilarNetworkTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);

        var inptToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var inptToOut = pool.SafeCreateConnectionGene(inp.nodeId, output.nodeId);

        NeuralNetwork large = new(pool, true, pool.connectGenes);
        NeuralNetwork small = new (pool, true, [inptToHidden, HiddenToOut, inptToOut]);

        Selector selector = new();

        Assertions.AssertThat(selector.Compare(large, small)).IsLess(Selector.threshold);
    }

    [TestCase]
    public void CompareDifferentNetworkTest(){
        GenePool pool = new();
        var inp = pool.CreateNode(NodeGene.Type.INPUT);
        var inp2 = pool.CreateNode(NodeGene.Type.INPUT);
        var hidden = pool.CreateNode(NodeGene.Type.HIDDEN);
        var hidden2 = pool.CreateNode(NodeGene.Type.HIDDEN);
        var output = pool.CreateNode(NodeGene.Type.OUTPUT);


        var inpToHidden = pool.SafeCreateConnectionGene(inp.nodeId, hidden.nodeId);
        var inpToHidden2 = pool.SafeCreateConnectionGene(inp.nodeId, hidden2.nodeId);

        var inp2ToHidden = pool.SafeCreateConnectionGene(inp2.nodeId, hidden.nodeId);
        var inp2ToHidden2 = pool.SafeCreateConnectionGene(inp2.nodeId, hidden2.nodeId);

        var HiddenToOut = pool.SafeCreateConnectionGene(hidden.nodeId, output.nodeId);
        var Hidden2ToOut = pool.SafeCreateConnectionGene(hidden2.nodeId, output.nodeId);
        var inpToOut = pool.SafeCreateConnectionGene(inp.nodeId, output.nodeId);


        NeuralNetwork small = new(pool, true, [inpToOut]);
        NeuralNetwork large = new(pool, true, [inpToHidden, inp2ToHidden2, inp2ToHidden, inpToHidden2, HiddenToOut, Hidden2ToOut]);

        Selector selector = new();
        Assertions.AssertThat(selector.Compare(small, large)).IsGreater(Selector.threshold);
    }

}