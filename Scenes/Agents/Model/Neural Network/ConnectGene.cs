using System;

public class ConnectGene{
    public int inGene;
    public int outGene;
    public double weight;
    public bool enabled;
    public int innovation;

    private static int InnovationCount = 0;
    private static readonly Random random= new Random();

    public ConnectGene(int inGene, int outGene){
        this.inGene = inGene;
        this.outGene = outGene;
        this.weight = random.NextDouble() * random.Next(-10, 10);
        this.innovation = InnovationCount;
        InnovationCount++;
    }

    public long Hash(){
        var A = (ulong)(inGene >= 0 ? 2 * (long)inGene : -2 * (long)inGene - 1);
        var B = (ulong)(outGene >= 0 ? 2 * (long)outGene : -2 * (long)outGene - 1);
        var C = (long)((A >= B ? A * A + A + B : A + B * B) / 2);
        return inGene < 0 && outGene < 0 || inGene >= 0 && outGene >= 0 ? C : -C - 1;
    }

    public static long Hash(int inp, int outp){
        var A = (ulong)(inp >= 0 ? 2 * (long)inp : -2 * (long)inp - 1);
        var B = (ulong)(outp >= 0 ? 2 * (long)outp : -2 * (long)outp - 1);
        var C = (long)((A >= B ? A * A + A + B : A + B * B) / 2);
        return inp < 0 && outp < 0 || inp >= 0 && outp >= 0 ? C : -C - 1;
    }
}