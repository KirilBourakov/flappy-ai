using System;
using System.Collections.Generic;

namespace NEAT{
    public class NeuralNetwork{
        public GenePool pool;
        public int fitness;
        public List<ConnectGene> structure = new();
        // TODO: create a structure sorted by innovation number to avoid calculation when crossover occurs.
        public bool hasBias;
        private readonly Random random = new();

        public static readonly int WEIGHT_MUTATION_RATE = 80;
        public static readonly int REPLACEMENT_RATE = 5;
        public static readonly double MAX_WEIGHT_CHANGE = 0.2;
        public static readonly int FLIP_ACTIVITY_RATE = 5;
        public static readonly int ADD_CONNECTION_RATE = 5;
        public static readonly int ADD_NODE_RATE = 3;


        public NeuralNetwork(GenePool pool, bool hasBias, List<ConnectGene> structure){
            this.pool = pool;
            this.hasBias = hasBias;
            this.structure = structure;
        }
        public NeuralNetwork(GenePool pool, int inputs, int outputs, bool useBias){
            if (inputs <= 0 || outputs <= 0){
                throw new Exception("Invalid input or output decleration");
            }

            this.hasBias = useBias;
            this.pool = pool;

            if (useBias){
                inputs++;
            }
            var inputNodes = this.pool.getGeneByType(NodeGene.Type.INPUT);
            if (inputNodes.Count == 0){
                for (int i = 0; i<inputs; i++){
                    this.pool.CreateNode(NodeGene.Type.INPUT);
                }
            }
            var outputNodes = this.pool.getGeneByType(NodeGene.Type.OUTPUT);
            if (outputNodes.Count==0){
                for (int i = 0; i < outputs; i++){
                    this.pool.CreateNode(NodeGene.Type.OUTPUT);
                }
            }
        }

        /// <summary>
        /// Runs the neural network for a set of inputs 
        /// </summary>
        /// <param name="inpt">A list of doubles representing the input.</param>
        /// <returns>The outputs of the network</returns>
        public double[] Evaluate(double[] inpt){
            // prepare nodes for calculation
            this.pool.ClearLayer(NodeGene.Type.OUTPUT);
            this.pool.ClearLayer(NodeGene.Type.HIDDEN);
            for (int i = 0; i < inpt.Length; i++)
            {
                pool.SafeGetNode(i, NodeGene.Type.INPUT).Value = inpt[i];
            }
            if (hasBias){
                pool.SafeGetNode(inpt.Length, NodeGene.Type.INPUT).Value = 1;
            }

            // run through the hidden layer connections (list assumed to be topologically sorted)
            // TODO: NEED to sort list topologically
            foreach (var connection in structure){
                if (connection.enabled){
                    NodeGene inp = pool.SafeGetNode(connection.inGene);
                    NodeGene outp = pool.SafeGetNode(connection.outGene);
                    outp.Value += connection.weight * inp.Value;
                }
            }

            // get the values
            var output = this.pool.getGeneByType(NodeGene.Type.OUTPUT);
            double[] result = new double[output.Count];
            for (int i = 0; i < output.Count; i++){
                result[i] = output[i].Value;
            }

            return result;
        }

        /// <summary>
        /// Given an NeuralNetwork, creates a new child NeuralNeywork
        /// </summary>
        /// <param name="other">The other NeuralNetwork</param>
        /// <returns>A new NeuralNetwork</returns>
        public NeuralNetwork Crossover(NeuralNetwork other){
            var newStructure = new List<ConnectGene>();

            // get the structure of the parent with the highest and lowest fitness (chosen randomly if the parents have the same fitness)
            List<ConnectGene> highestFitness;
            List<ConnectGene> lowestFitness;
            if (this.fitness > other.fitness || (this.fitness == other.fitness && this.random.Next(0,2) == 0)){
                highestFitness = new List<ConnectGene>(this.structure);
                lowestFitness = new List<ConnectGene>(other.structure);
            } else {
                highestFitness = new List<ConnectGene>(other.structure);
                lowestFitness = new List<ConnectGene>(this.structure);
            }
            
            // sort by inovation
            highestFitness.Sort((x, y) => x.innovation.CompareTo(y.innovation));
            lowestFitness.Sort((x, y) => x.innovation.CompareTo(y.innovation));


            // cross over
            bool done = false;
            int i = 0;
            while (!done){
                
                var currHighGene = (i >= 0 && i < highestFitness.Count) ? highestFitness[i] : null;
                var currLowGene = (i >= 0 && i < lowestFitness.Count) ? lowestFitness[i] : null;
                
                // if i does not exist somewhere we have entered excess genes 
                if (currHighGene == null){
                    done = true;
                }
                else if (currLowGene == null){
                    int j = i+1;
                    while (currHighGene != null){
                        newStructure.Add(currHighGene.Copy());
                        currHighGene = (j >= 0 && j < highestFitness.Count) ? highestFitness[j] : null;;
                        j++;
                    }
                    done = true;
                }
                // innovations match
                else if (currHighGene.innovation == currLowGene.innovation){
                    if (this.random.Next(0,2) == 0){
                        newStructure.Add(currHighGene.Copy());
                    } else {
                        newStructure.Add(currLowGene.Copy());
                    }
                }
                // disjoint
                else if (currHighGene.innovation < currLowGene.innovation){
                    newStructure.Add(currHighGene.Copy());
                }
                else if (currLowGene.innovation < currHighGene.innovation){
                    newStructure.Add(currLowGene.Copy());
                }
                i++;
            }
            

            // TODO: blend genes when fitness is the same
            // mutation
            i = 0;
            foreach (var connection in newStructure){
                int roll = MutationRoll();
                if (roll <= WEIGHT_MUTATION_RATE){
                    // replace or mutate weight

                        if (roll <= REPLACEMENT_RATE){
                        connection.weight = random.Next(-2, 3) * random.NextDouble();
                    }
                    else{
                        connection.weight += random.Next(-1, 2) * MAX_WEIGHT_CHANGE;
                    }

                    // flip node   
                    if (MutationRoll() <= FLIP_ACTIVITY_RATE){
                        connection.enabled = !connection.enabled;
                    }

                    // create a new connection
                    if (MutationRoll() <= ADD_CONNECTION_RATE){
                        int inp = connection.inGene;
                        int target_count = GenePool.genesByType[NodeGene.Type.INPUT].Count + GenePool.genesByType[NodeGene.Type.OUTPUT].Count;
                        if (inp < target_count){
                            int outp = random.Next(inp+1, target_count+1);
                            ConnectGene newConnection = this.pool.SafeCreateConnectionGene(inp, outp).Copy();
                            newConnection.weight = random.NextDouble() * random.Next(-2, 3);
                        }
                    }

                    // create a new node
                    if (MutationRoll() <= ADD_NODE_RATE){
                        connection.enabled = false;
                        NodeGene newNode = this.pool.CreateNode(NodeGene.Type.HIDDEN);
                        ConnectGene toNew = this.pool.SafeCreateConnectionGene(connection.inGene, newNode.nodeId).Copy();
                        toNew.weight = 1;
                        ConnectGene fromNew = this.pool.SafeCreateConnectionGene(newNode.nodeId, connection.outGene).Copy();
                        fromNew.weight = connection.weight;
                        structure.Add(toNew);
                        structure.Add(fromNew);
                    }
                }
            

                i++;
            }


            return new NeuralNetwork(this.pool, this.hasBias, newStructure);
        }

        /// <summary>
        /// Chooses a random precentage between 0-100
        /// </summary>
        /// <returns>An int between 0-100</returns>
        private int MutationRoll(){
            return this.random.Next(0, 101);
        }
    }
}