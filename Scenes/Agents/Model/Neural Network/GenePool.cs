using System;
using System.Collections.Generic;

namespace NEAT
{
    public class GenePool{
        public static List<NodeGene> genes = new List<NodeGene>();
        public static Dictionary<int, NodeGene> geneById = new Dictionary<int, NodeGene>();

        public static List<ConnectGene> connectGene = new List<ConnectGene>();
        public static Dictionary<long, ConnectGene> connectGeneByHash = new Dictionary<long, ConnectGene>();
        

        public List<NodeGene> getGeneByType(NodeGene.Type type){
            List<NodeGene> inputs = new List<NodeGene>();
            foreach(NodeGene inputGene in genes){
                if (inputGene.nodeType == type){
                    inputs.Add(inputGene);
                }
            }
            return inputs;
        }

        public NodeGene CreateNode(NodeGene.Type type){
            NodeGene newNode = new(type);
            genes.Add(newNode);
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

    }
}