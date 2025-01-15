using GdUnit4;
using Godot;
using NEAT;

namespace Test;

[TestSuite]
public class NodeGeneTest
{
    [TestCase]
    public void ConstructorTest(){
        NodeGene gene = new NodeGene(NodeGene.Type.INPUT, 0);
        NodeGene gene2 = new NodeGene(NodeGene.Type.HIDDEN, 1);
        Assertions.AssertInt(gene.nodeId).IsEqual(1);
        Assertions.AssertInt(gene2.nodeId).IsEqual(2);
    }
}