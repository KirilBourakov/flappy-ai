using System;
using System.Collections.Generic;
using Godot;

namespace NEAT
{
    public partial class NeuralNetwork{
        /// <summary>
        /// Given an NeuralNetwork, creates a new child NeuralNeywork
        /// </summary>
        /// <param name="other">The other NeuralNetwork</param>
        /// <returns>A new NeuralNetwork</returns>
        
        // todo: crossover should copy nodeById. 
        public NeuralNetwork Crossover(NeuralNetwork other){
            if (other == null){
                throw new ArgumentNullException("Cannot Reproduce with other");
            }
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
                        newStructure.Add(currHighGene.Clone());
                        currHighGene = (j >= 0 && j < highestFitness.Count) ? highestFitness[j] : null;;
                        j++;
                    }
                    done = true;
                }
                // innovations match
                else if (currHighGene.innovation == currLowGene.innovation){
                    if (this.random.Next(0,2) == 0){
                        newStructure.Add(currHighGene.Clone());
                    } else {
                        newStructure.Add(currLowGene.Clone());
                    }
                }
                // disjoint
                else if (currHighGene.innovation < currLowGene.innovation){
                    newStructure.Add(currHighGene.Clone());
                }
                else if (currLowGene.innovation < currHighGene.innovation){
                    newStructure.Add(currLowGene.Clone());
                }
                i++;
            }
            

            // TODO: blend genes when fitness is the same
            // mutation
            Mutate(newStructure);
            
            return new NeuralNetwork(this.pool, this.hasBias, newStructure);
        }

        private void Mutate(List<ConnectGene> newStructure){
            int i = 0;
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
                        int target_count = pool.genesByType[NodeGene.Type.INPUT].Count + pool.genesByType[NodeGene.Type.OUTPUT].Count;
                        if (inp < target_count){
                            int outp = random.Next(inp+1, target_count+1);
                            ConnectGene newConnection = this.pool.SafeCreateConnectionGene(inp, outp).Clone();
                            newConnection.weight = random.NextDouble() * random.Next(-2, 3);
                        }
                    }

                    // create a new node
                    if (MutationRoll() <= ADD_NODE_RATE){
                        connection.enabled = false;
                        NodeGene newNode = this.pool.CreateNode(NodeGene.Type.HIDDEN);
                        ConnectGene toNew = this.pool.SafeCreateConnectionGene(connection.inGene, newNode.nodeId).Clone();
                        toNew.weight = 1;
                        ConnectGene fromNew = this.pool.SafeCreateConnectionGene(newNode.nodeId, connection.outGene).Clone();
                        fromNew.weight = connection.weight;
                        structure.Add(toNew);
                        structure.Add(fromNew);
                    }
                }
                i++;
            }
        }
        
        // TODO: should recive a child neural network and modify that, not a structure
        /// <summary>
        /// Flips a random connections enabled status within the network
        /// </summary>
        /// <param name="network"></param>
        private static void FlipRandomConnection(NeuralNetwork network){
            ConnectGene chosen = network.structure[network.random.Next(0, network.structure.Count)];
            chosen.enabled = !chosen.enabled;
        }

        /// <summary>
        /// Adds a connection between two random nodes in the network
        /// </summary>
        /// <param name="network"></param>
        private static void AddConnection(NeuralNetwork network){
            var nodes = new List<NodeGene>(network.nodeById.Values);
            var inp = nodes[network.random.Next(0, nodes.Count)];

            NodeGene outp;
            int attempts = 0;
            do {
                outp = nodes[network.random.Next(0, nodes.Count)];
                attempts++;
            } while (attempts < 20 && inp.Layer >= outp.Layer);

            if (inp.Layer < outp.Layer){
                network.structure.Add(network.pool.SafeCreateConnectionGene(inp.nodeId, outp.nodeId));
            }
        }

        /// <summary>
        /// Adds a node inbetween a random neural network connection
        /// </summary>
        /// <param name="network"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void AddNode(NeuralNetwork network){
            // TODO: consider only adding nodes on enabled connections
            ConnectGene chosen = network.structure[network.random.Next(0, network.structure.Count)];
            chosen.enabled = false;

            NodeGene inp = network.nodeById[chosen.inGene];
            NodeGene outp = network.nodeById[chosen.outGene];

            int newNodeLayer = inp.nodeId + 1;
            NodeGene newNode = network.pool.CreateNode(NodeGene.Type.HIDDEN, newNodeLayer);
            network.nodeById[newNode.nodeId] = newNode;

            // if output and input are right beside eachother, a new layer is created
            if (outp.nodeId - inp.nodeId == 1){
                foreach (KeyValuePair<int, NodeGene> entry in network.nodeById)
                {
                    if (entry.Value.Layer >= newNodeLayer){
                        entry.Value.Layer++;
                    }
                }
            }
            if (outp.nodeId <= inp.nodeId){
                throw new InvalidOperationException($"Reccurent connection; inp node {inp.nodeId} >= out node {outp.nodeId}");
            }
            // create connections
            var fromInpToNew = network.pool.SafeCreateConnectionGene(inp.nodeId, newNode.nodeId);
            fromInpToNew.weight = chosen.weight;
            var fromNewToOut = network.pool.SafeCreateConnectionGene(newNode.nodeId, outp.nodeId);
            network.structure.Add(fromInpToNew);
            network.structure.Add(fromNewToOut);
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