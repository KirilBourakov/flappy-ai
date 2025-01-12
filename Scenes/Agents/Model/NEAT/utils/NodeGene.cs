using System;

namespace NEAT
{
    public class NodeGene{
    public enum Type {INPUT, HIDDEN, OUTPUT};
    
    // todo: add activation function on input
    public double Value {get; set;}
    public Type nodeType {get;}
    public int nodeId {get;}
    public int layer {get;}

    private static int IdCounter = 1;

    public NodeGene(Type nodeType, int layer){
        this.Value = 0;
        this.layer = layer;
        this.nodeType = nodeType;
        this.nodeId = IdCounter;
        IdCounter++;
    }
    
}
}
