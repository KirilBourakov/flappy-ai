using System;

namespace NEAT
{
    public class NodeGene{
    public enum Type {INPUT, HIDDEN, OUTPUT};
    
    // todo: add activation function on input
    // todo: fix encapsulation
    public double Value {get; set;}
    public readonly Type nodeType;
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
    
    /// <summary>
    /// Creates a shallow copy of the NodeGene
    /// </summary>
    /// <returns></returns>
    public NodeGene Clone(){
        return (NodeGene)this.MemberwiseClone();
    }
}
}
