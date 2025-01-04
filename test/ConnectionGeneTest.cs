using GdUnit4;
using NEAT;

[TestSuite]
public class ConnectGeneTest
{
    [TestCase]
    public void CopyTest(){
        var gene = new ConnectGene(1, 2);
        var gene2 = gene.Copy();

        Assertions.AssertObject(gene).IsNotSame(gene2);
        Assertions.AssertThat(gene.Hash()).IsEqual(gene2.Hash());
    }

    [TestCase]
    public void HashTest(){
        var gene = new ConnectGene(1, 2);

        Assertions.AssertThat(gene.Hash()).IsEqual(ConnectGene.Hash(1,2));
        Assertions.AssertThat(gene.Hash()).IsNotEqual(ConnectGene.Hash(2,1));
    }
}