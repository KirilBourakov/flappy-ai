using GdUnit4;
using NEAT;

[TestSuite]
public class GenePoolTest
{
   [TestCase]
   public void GetGeneByTypeTest() {
        GenePool pool = new GenePool();
        const int INPUT_SIZE = 3;
        const int HIDDEN_SIZE = 10;
        const int OUTPUT_SIZE = 2;
        for (int i = 0; i < INPUT_SIZE; i++){
            GenePool.genesByType[NodeGene.Type.INPUT].Add(new NodeGene(NodeGene.Type.INPUT));
        }
        for (int i = 0; i < HIDDEN_SIZE; i++)
        {
            GenePool.genesByType[NodeGene.Type.HIDDEN].Add(new NodeGene(NodeGene.Type.HIDDEN));
        }
        for (int i = 0; i < OUTPUT_SIZE; i++){
            GenePool.genesByType[NodeGene.Type.OUTPUT].Add(new NodeGene(NodeGene.Type.OUTPUT));
        }

        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.INPUT).Count).Equals(INPUT_SIZE);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.HIDDEN).Count).Equals(HIDDEN_SIZE);
        Assertions.AssertInt(pool.getGeneByType(NodeGene.Type.OUTPUT).Count).Equals(OUTPUT_SIZE);
   }
}
