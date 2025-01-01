using System;
using System.Collections.Generic;
using Godot;

namespace NEAT
{
    public class GenePool{
        public static Dictionary<NodeGene.Type, List<NodeGene>> genesByType = new Dictionary<NodeGene.Type, List<NodeGene>> {
            {NodeGene.Type.INPUT, new List<NodeGene>()},        
            {NodeGene.Type.HIDDEN, new List<NodeGene>()},
            {NodeGene.Type.OUTPUT, new List<NodeGene>()}
        };
        public static Dictionary<int, NodeGene> geneById = new();

        public static List<ConnectGene> connectGene = new();
        public static Dictionary<long, ConnectGene> connectGeneByHash = new();
        

        public List<NodeGene> getGeneByType(NodeGene.Type type){
            if (!genesByType.TryGetValue(type, out List<NodeGene> layer)){
                throw new Exception("Invalid Type: " + type);
            }
            return layer;
        }

        public NodeGene CreateNode(NodeGene.Type type){
            NodeGene newNode = new(type);
            if (!genesByType.TryGetValue(type, out List<NodeGene> layer)){
                throw new Exception("Invalid Type: " + type);
            }
            layer.Add(newNode);
            geneById.Add(newNode.nodeId, newNode);
            return newNode;
        }

        public NodeGene SafeGetNode(int id, NodeGene.Type expected){
            if (!geneById.TryGetValue(id, out NodeGene value)){
                    throw new Exception("Failure to find node in Evaluate");
            }   
            if (value.nodeType != expected){
                throw new Exception("Node found not of type INPUT");
            }

            return value;
        }
        public NodeGene SafeGetNode(int id){
            if (!geneById.TryGetValue(id, out NodeGene value)){
                    throw new Exception("Failure to find node in Evaluate");
            }   
            return value;
        }

        public ConnectGene SafeCreateConnectionGene(int inp, int outp){
            ConnectGene connection;
            if(connectGeneByHash.ContainsKey(ConnectGene.Hash(inp, outp))){
                bool success = connectGeneByHash.TryGetValue(ConnectGene.Hash(inp, outp), out connection);
                if (!success){
                    throw new Exception("TryGetValue failed.");
                }
            } else {
                connection = new(inp, outp);
                connectGeneByHash.Add(connection.Hash(), connection);
            }  
            return connection;
        }

        public void ClearLayer(NodeGene.Type type){
            var layer = getGeneByType(type);
            foreach (var node in layer)
            {
                node.Value = 0;
            }
        }
    }
}