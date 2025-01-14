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


    private int _layer;
    public int Layer {
        get => _layer; 
        set {
            if (value - _layer == 1){
                _layer = value;
            } else {
                throw new InvalidOperationException($"{nodeType} Layer {nodeId} is being moved more then 1 layer to {value}");
            }
        } 
    }

    private static int IdCounter = 1;

    public NodeGene(Type nodeType, int layer){
        this.Value = 0;
        this._layer = layer;
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
