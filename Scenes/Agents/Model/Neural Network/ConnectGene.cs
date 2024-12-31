using System;
using Godot;

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
}