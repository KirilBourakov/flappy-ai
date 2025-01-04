using GdUnit4;
using Godot;
using NEAT;

[TestSuite]
public class NodeGeneTest
{
    [TestCase]
    public void ConstructorTest(){
        NodeGene gene = new NodeGene(NodeGene.Type.INPUT);
        NodeGene gene2 = new NodeGene(NodeGene.Type.HIDDEN);
        Assertions.AssertInt(gene.nodeId).Equals(1);
        Assertions.AssertInt(gene2.nodeId).Equals(2);
    }
}