using GdUnit4;
using NEAT;

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
            pool.genesByType[NodeGene.Type.INPUT].Add(new NodeGene(NodeGene.Type.INPUT));
        }
        for (int i = 0; i < HIDDEN_SIZE; i++)
        {
            pool.genesByType[NodeGene.Type.HIDDEN].Add(new NodeGene(NodeGene.Type.HIDDEN));
        }
        for (int i = 0; i < OUTPUT_SIZE; i++){
            pool.genesByType[NodeGene.Type.OUTPUT].Add(new NodeGene(NodeGene.Type.OUTPUT));
        }

        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.INPUT).Count).Equals(INPUT_SIZE);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.HIDDEN).Count).Equals(HIDDEN_SIZE);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.OUTPUT).Count).Equals(OUTPUT_SIZE);
    }

    [TestCase]
    public void CreateNodeTest(){
        GenePool pool = new();
        var gene = pool.CreateNode(NodeGene.Type.INPUT);
        Assertions.AssertObject(pool.geneById[gene.nodeId]).IsSame(gene);
        Assertions.AssertObject(pool.genesByType[NodeGene.Type.INPUT][0]).IsSame(gene);
    }

    [TestCase]
    public void SafeGetNodeReturnsGeneTest(){
        GenePool pool = new();
        var gene = pool.CreateNode(NodeGene.Type.INPUT);
        var response = pool.SafeGetNode(gene.nodeId, NodeGene.Type.INPUT);
        var response2 = pool.SafeGetNode(gene.nodeId);

        Assertions.AssertObject(response).IsSame(gene);
        Assertions.AssertObject(response2).IsSame(gene);
    }

    [TestCase]
    public void SafeGetNodeErrorsTest(){
        GenePool pool = new();
        var gene = pool.CreateNode(NodeGene.Type.INPUT);

        Assertions.AssertThrown(() => pool.SafeGetNode(gene.nodeId, NodeGene.Type.HIDDEN));
    }

    [TestCase]
    public void SafeCreateConnectionGeneCreatesTest(){
        GenePool pool = new();
        var newConnection = pool.SafeCreateConnectionGene(1,2);
        Assertions.AssertObject(pool.connectGenes[0]).IsSame(newConnection);
        Assertions.AssertObject(pool.connectGenesByHash[newConnection.Hash()]).IsSame(newConnection);
    }

    [TestCase]
    public void SafeCreateConnectionGene(){
        GenePool pool = new();
        var newConnection = pool.SafeCreateConnectionGene(1,2);
        var copyConnection = pool.SafeCreateConnectionGene(1,2);
        Assertions.AssertObject(newConnection).IsSame(copyConnection);
    }

}
