using System;
using System.Collections.Generic;
using Godot;

namespace NEAT{
    public partial class NeuralNetwork{
        public GenePool pool {get;}
        public double fitness;
        public double adjustedFitness;
        public double relativeFitness;

        public Dictionary<int, NodeGene> nodeById = new();
        public List<ConnectGene> structure {get; private set;} = new();
        // TODO: create a structure sorted by innovation number to avoid calculation when crossover occurs.
        public bool hasBias {get;}

        /// <summary>
        /// Create a neural network with a preset structure
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="hasBias"></param>
        /// <param name="structure"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public NeuralNetwork(GenePool pool, bool hasBias, List<ConnectGene> structure){
            if (structure == null){
                throw new ArgumentNullException("structure cannot be null");
            }
            List<ConnectGene> copy = [];
            foreach (var item in structure)
            {
                copy.Add(item.Clone());
            }
            this.pool = pool;
            this.hasBias = hasBias;
            this.structure = copy;
        }

        /// <summary>
        /// Create a NeuralNetwork with a given number of inputs and outputs
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <param name="useBias"></param>
        /// <exception cref="Exception"></exception>
        public NeuralNetwork(GenePool pool, int inputs, int outputs, bool useBias){
            if (inputs <= 0 || outputs <= 0){
                throw new Exception("Invalid input or output decleration");
            }

            this.hasBias = useBias;
            this.pool = pool;

            if (useBias){
                inputs++;
            }
            List<NodeGene> createLayer(NodeGene.Type type, int amount){
                var nodes = this.pool.getGeneByType(type);
                if (nodes.Count == 0){
                    for (int i = 0; i<amount; i++){
                        NodeGene node = this.pool.CreateNode(type, type == NodeGene.Type.INPUT ? 1 : 2);
                        nodeById[node.nodeId] = node;
                        nodes.Add(node);
                    }
                } else {
                    foreach (var node in nodes)
                    {
                        nodeById[node.nodeId] = node;
                    }
                }
                return nodes;
            }
            var inputNodes = createLayer(NodeGene.Type.OUTPUT, inputs);
            var outputNodes = createLayer(NodeGene.Type.OUTPUT, outputs);

            //TODO: replace starting with a fully connected node with smarter evolution
            for (int i = 0; i < inputNodes.Count; i++){
                for (int j = 0; j < outputNodes.Count; j++){
                    this.structure.Add(pool.SafeCreateConnectionGene(inputNodes[i].nodeId, outputNodes[j].nodeId));
                }
            }  
        }

        /// <summary>
        /// Runs the neural network for a set of inputs 
        /// </summary>
        /// <param name="inpt">A list of doubles representing the input.</param>
        /// <returns>The outputs of the network</returns>
        public double[] Evaluate(double[] inpt){
            // clear values
            foreach (KeyValuePair<int, NodeGene> entry in nodeById)
            {
                entry.Value.Value = 0;
            }

            int getConnectionInpInt(ConnectGene inp){
                return nodeById[inp.inGene].layer;
            }
            structure.Sort((a,b) => getConnectionInpInt(a).CompareTo(getConnectionInpInt(b)));

            int inptLocation = 0;
            foreach (var connction in structure)
            {
                NodeGene inp = nodeById[connction.inGene];
                NodeGene outp = nodeById[connction.outGene];

                if (inp.nodeType == NodeGene.Type.INPUT){
                    inp.Value = (inptLocation >= 0 && inptLocation < inpt.Length) ? inpt[inptLocation] : 1;
                    inptLocation = 1;
                }

                outp.Value += inp.Value * connction.weight;
            }

            //TODO: optimize
            // Get a list of all the outputs, and order them by id (so they never change)
            List<NodeGene> outputs = [];
            foreach (KeyValuePair<int, NodeGene> entry in nodeById)
            {
                if (entry.Value.nodeType == NodeGene.Type.OUTPUT){
                    outputs.Add(entry.Value);
                }
            }
            outputs.Sort((a,b) => a.nodeId.CompareTo(b.nodeId));

            double[] result = new double[outputs.Count];
            for (int i = 0; i < outputs.Count; i++)
            {
                result[i] = outputs[i].Value;
            }

            return result;
        }

        // TODO: move this method to the selector class
        public ConnectGene NextGene(ref int currIndex, ref int currSize){
            ConnectGene currGene;
            do {
                currIndex++;
                currGene = (currIndex >= 0 && currIndex < this.structure.Count) ? this.structure[currIndex] : null;
            }  while (currGene != null && !currGene.enabled);

            if (currGene != null){
                currSize++;
            }
            return currGene;
        }

        /// <summary>
        /// Topologically sorts the neural networks structure
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void TopologicSort(){
            int count = structure.Count;
            bool[] visited = new bool[count];
            List<ConnectGene> ordered = new List<ConnectGene>();
            for (int k = 0; k < count; k++) {
                ordered.Add(null);
            }
            int i = count-1;

            for (int at = 0; at < count; at++){
                if (!visited[at]){
                    i = dfs(i, at, visited, ordered);
                }
            }
            if (ordered.Contains(null)) {
                throw new InvalidOperationException("Topological sorting failed.");
            }

            this.structure = ordered;
        }   
        /// <summary>
        /// DFS helper for the TopologicSort method
        /// </summary>
        private int dfs(int i, int at, bool[] visited, List<ConnectGene> ordered){
            visited[at] = true;
        
            List<ConnectGene> edges = this.GetStructuralConnections(structure[at], out List<int> trueIndexs);
            int j=0;
            foreach(var edge in edges){
                if(!visited[trueIndexs[j]]){
                    i = dfs(i, trueIndexs[j], visited, ordered);
                }
                j++;
            }

            ordered[i] = structure[at];
            return i-1;
        }

        /// <summary>
        /// Gets the ConnectGene that a certain target leads to.
        /// </summary>
        /// <param name="target">The target whos outgene you want to search</param>
        /// <param name="trueIndexs">The indexs of each target within the structure array</param>
        /// <returns>A list of ConnectGenes that the input leads to.</returns>
        private List<ConnectGene> GetStructuralConnections(ConnectGene target, out List<int> trueIndexs){
            List<ConnectGene> output = new();
            trueIndexs = new();

            int i = 0;
            foreach(ConnectGene test in structure){
                if(test.inGene == target.outGene){
                    output.Add(test);
                    trueIndexs.Add(i);
                }
                i++;
            }
            return output;
        }
    }
}