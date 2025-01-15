using GdUnit4;
using NEAT;

namespace Test;

[TestSuite]
public class GenePoolTest
{
    [TestCase]
    public void GetGeneByTypeTest() {
        GenePool pool = new();
        const int INPUT_SIZE = 3;
        const int HIDDEN_SIZE = 10;
        const int OUTPUT_SIZE = 2;
        for (int i = 0; i < INPUT_SIZE; i++){
            pool.genesByType[NodeGene.Type.INPUT].Add(new NodeGene(NodeGene.Type.INPUT, 0));
        }
        for (int i = 0; i < HIDDEN_SIZE; i++)
        {
            pool.genesByType[NodeGene.Type.HIDDEN].Add(new NodeGene(NodeGene.Type.HIDDEN, 1));
        }
        for (int i = 0; i < OUTPUT_SIZE; i++){
            pool.genesByType[NodeGene.Type.OUTPUT].Add(new NodeGene(NodeGene.Type.OUTPUT, 2));
        }

        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.INPUT).Count).IsEqual(INPUT_SIZE);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.HIDDEN).Count).IsEqual(HIDDEN_SIZE);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.OUTPUT).Count).IsEqual(OUTPUT_SIZE);
    }

    [TestCase]
    public void CreateNodeTest(){
        GenePool pool = new();
        var gene = pool.CreateNode(NodeGene.Type.INPUT, 0);
        Assertions.AssertInt(pool.geneById[gene.nodeId].nodeId).IsEqual(gene.nodeId);
        Assertions.AssertInt(pool.genesByType[NodeGene.Type.INPUT][0].nodeId).IsEqual(gene.nodeId);
    }

    [TestCase]
    public void SafeGetNodeReturnsGeneTest(){
        GenePool pool = new();
        var gene = pool.CreateNode(NodeGene.Type.INPUT, 0);
        var response = pool.SafeGetNode(gene.nodeId, NodeGene.Type.INPUT);
        var response2 = pool.SafeGetNode(gene.nodeId);

        Assertions.AssertInt(response.nodeId).IsEqual(gene.nodeId);
        Assertions.AssertInt(response2.nodeId).IsEqual(gene.nodeId);
    }

    [TestCase]
    public void SafeGetNodeErrorsTest(){
        GenePool pool = new();
        var gene = pool.CreateNode(NodeGene.Type.INPUT, 0);

        Assertions.AssertThrown(() => pool.SafeGetNode(gene.nodeId, NodeGene.Type.HIDDEN));
    }

    [TestCase]
    public void SafeCreateConnectionGeneCreatesTest(){
        GenePool pool = new();
        var newConnection = pool.SafeCreateConnectionGene(1,2);

        Assertions.AssertInt(pool.connectGenes[0].innovation).IsEqual(newConnection.innovation);
        Assertions.AssertInt(pool.connectGenesByHash[newConnection.Hash()].innovation).IsEqual(newConnection.innovation);
    }

    [TestCase]
    public void SafeCreateConnectionGene(){
        GenePool pool = new();
        var newConnection = pool.SafeCreateConnectionGene(1,2);
        var copyConnection = pool.SafeCreateConnectionGene(1,2);
        Assertions.AssertInt(newConnection.innovation).IsEqual(copyConnection.innovation);
    }

}
