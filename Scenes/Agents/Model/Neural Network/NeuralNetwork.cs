using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace NEAT{
    public class NeuralNetwork{
        public static List<NodeGene> genes = new List<NodeGene>();
        public static Dictionary<int, NodeGene> geneById = new Dictionary<int, NodeGene>();
        public static List<ConnectGene> connectGene = new List<ConnectGene>();
        public static Dictionary<long, ConnectGene> connectGeneByHash = new Dictionary<long, ConnectGene>();

        public List<ConnectGene> structure = new List<ConnectGene>();
        public bool hasBias;


        public NeuralNetwork(int inputs, int outputs, bool useBias){
            this.hasBias = useBias;

            int lastInp = -1;
            int lastOutp = -1;
            if (genes.Count == 0){
                for (int i = 0; i < inputs; i++) {
                    NodeGene newNode = CreateNode(NodeGene.Type.INPUT);
                    lastInp = newNode.nodeId;
                }
                if (useBias){
                    CreateNode(NodeGene.Type.INPUT);
                    NodeGene newNode = CreateNode(NodeGene.Type.INPUT);
                    lastInp = newNode.nodeId;
                }

                for (int i = 0; i < outputs; i++) {
                    NodeGene newNode = CreateNode(NodeGene.Type.OUTPUT);
                    lastOutp = newNode.nodeId;
                }
            } else {
                lastInp = inputs + (useBias ? 1 : 0);
                lastOutp = lastInp + outputs;
            }
            if (lastInp == -1 || lastOutp == -1){
                throw new Exception($"Failure to generate initial structure; lastInp: {lastInp}, lastOutp: {lastOutp}");
            }

            ConnectGene gene = SafeCreateGene(lastInp,lastOutp);
            this.structure.Add(gene);
        }

        public double[] Evaluate(double[] inpt){
            for (int i = 0; i < inpt.Length; i++)
            {
                SafeGetNode(i).Value = inpt[i];
            }

            if (hasBias){
                SafeGetNode(inpt.Length).Value = 1;
            }

            
            
            return new double[] {1};
        }

        private NodeGene CreateNode(NodeGene.Type type){
            NodeGene newNode = new(type);
            genes.Add(newNode);
            geneById.Add(newNode.nodeId, newNode);
            return newNode;
        }
        private NodeGene SafeGetNode(int id){
            if (!geneById.TryGetValue(id, out NodeGene value)){
                    throw new Exception("Failure to find node in Evaluate");
            }   
            if (value.nodeType != NodeGene.Type.INPUT){
                throw new Exception("Node found not of type INPUT");
            }

            return value;
        }

        private ConnectGene SafeCreateGene(int inp, int outp){
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