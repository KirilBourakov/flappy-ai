using System;
using System.Collections.Generic;

namespace NEAT
{
    public class GenePool{
        /// <summary>
        /// A dictionary that stores each node gene by it's type
        /// </summary>
        public Dictionary<NodeGene.Type, List<NodeGene>> genesByType = new Dictionary<NodeGene.Type, List<NodeGene>> {
            {NodeGene.Type.INPUT, new List<NodeGene>()},        
            {NodeGene.Type.HIDDEN, new List<NodeGene>()},
            {NodeGene.Type.OUTPUT, new List<NodeGene>()}
        };
        /// <summary>
        /// A dictionary that stores each node gene by it's id.
        /// </summary>
        public Dictionary<int, NodeGene> geneById = new();

        /// <summary>
        /// A List that stores every connection gene
        /// </summary>
        public List<ConnectGene> connectGenes = new();
        /// <summary>
        /// A dictionary that stores each connection gene by it's hash.
        /// </summary>
        public Dictionary<long, ConnectGene> connectGenesByHash = new();
        

        /// <summary>
        /// Given a type, return every node of that type
        /// </summary>
        public List<NodeGene> getGeneByType(NodeGene.Type type){
            if (!genesByType.TryGetValue(type, out List<NodeGene> layer)){
                throw new Exception("Invalid Type: " + type);
            }
            return layer;
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        public NodeGene CreateNode(NodeGene.Type type){
            NodeGene newNode = new(type);
            if (!genesByType.TryGetValue(type, out List<NodeGene> layer)){
                throw new Exception("Invalid Type: " + type);
            }
            layer.Add(newNode);
            geneById.Add(newNode.nodeId, newNode);
            return newNode;
        }
        
        /// <summary>
        /// Gets a node by id, and ensures it is of an expected type
        /// </summary>
        public NodeGene SafeGetNode(int id, NodeGene.Type expected){
            if (!geneById.TryGetValue(id, out NodeGene value)){
                    throw new Exception("Failure to find node in Evaluate");
            }   
            if (value.nodeType != expected){
                throw new Exception("Node found not of type INPUT");
            }

            return value;
        }
        /// <summary>
        /// Gets a node by id.
        /// </summary>
        public NodeGene SafeGetNode(int id){
            if (!geneById.TryGetValue(id, out NodeGene value)){
                    throw new Exception("Failure to find node in Evaluate");
            }   
            return value;
        }

        /// <summary>
        /// Creates a connection gene between those an input and output, or returns the one that already exists.
        /// </summary>
        public ConnectGene SafeCreateConnectionGene(int inp, int outp){
            ConnectGene connection;
            if(connectGenesByHash.ContainsKey(ConnectGene.Hash(inp, outp))){
                bool success = connectGenesByHash.TryGetValue(ConnectGene.Hash(inp, outp), out connection);
                if (!success){
                    throw new Exception("TryGetValue failed.");
                }
            } else {
                connection = new(inp, outp);
                connectGenesByHash.Add(connection.Hash(), connection);
                connectGenes.Add(connection);
            }  
            return connection;
        }

        /// <summary>
        /// Given a layer, 0's it.
        /// </summary>
        public void ClearLayer(NodeGene.Type type){
            var layer = getGeneByType(type);
            foreach (var node in layer)
            {
                node.Value = 0;
            }
        }
    }
}