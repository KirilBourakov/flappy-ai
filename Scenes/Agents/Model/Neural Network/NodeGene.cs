using System;

namespace NEAT
{
    public class NodeGene{
    public enum Type {INPUT, HIDDEN, OUTPUT};
    private static int IdCounter = 1;
    public double Value;
    public Type nodeType;
    public int nodeId;

    public NodeGene(Type nodeType){
        this.Value = 0;
        this.nodeType = nodeType;
        this.nodeId = IdCounter;
        IdCounter++;
    }
}
}
