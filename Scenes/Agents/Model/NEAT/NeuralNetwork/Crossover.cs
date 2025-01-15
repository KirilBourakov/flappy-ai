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
        public NeuralNetwork Crossover(NeuralNetwork other){
            if (other == null) throw new ArgumentNullException("Cannot Reproduce with other");
            
            var newStructure = new List<ConnectGene>();

            // get the structure of the parent with the highest and lowest fitness (with preference towards smaller structures if fitness is even)
            // TODO: handle cast of equal fitness and size
            NeuralNetwork highestFitness;
            NeuralNetwork lowestFitness;
            if (this.fitness > other.fitness || (this.fitness == other.fitness && this.structure.Count < other.structure.Count)){
                highestFitness = this;
                lowestFitness = other;
            } else {
                highestFitness = other;
                lowestFitness = this;
            }
            
            // sort by inovation
            highestFitness.structure.Sort((x, y) => x.innovation.CompareTo(y.innovation));
            lowestFitness.structure.Sort((x, y) => x.innovation.CompareTo(y.innovation));

            // cross over
            int highPointer = 0;
            int lowPointer = 0;
            while (highPointer < highestFitness.structure.Count){
                var currHighGene = (highPointer >= 0 && highPointer < highestFitness.structure.Count) ? highestFitness.structure[highPointer].Clone() : null;
                var currLowGene = (lowPointer >= 0 && lowPointer < lowestFitness.structure.Count) ? lowestFitness.structure[lowPointer].Clone() : null;
                
                // excess genes from the fit parent should be copied
                if (currLowGene == null){
                    newStructure.Add(currHighGene);
                    highPointer++;
                }
                // choose random gene if innovations match
                else if (currLowGene.innovation == currHighGene.innovation){
                    newStructure.Add(
                        random.Next(0,2) == 1 ? currHighGene : currLowGene
                    );
                    lowPointer++;
                    highPointer++;
                }   
                // pass over lower parents disjoint
                else if (currLowGene.innovation < currHighGene.innovation){
                    lowPointer++;
                }
                // add high parents disjoint
                else if (currHighGene.innovation < currLowGene.innovation){
                    structure.Add(currHighGene);
                    highPointer++;
                }
                
            }   

            Dictionary<int, NodeGene> newNodesById = [];
            foreach (ConnectGene connection in newStructure)
            {
                foreach (int id in new int[] {connection.inGene, connection.outGene})
                {
                    bool success = highestFitness.nodeById.TryGetValue(id, out NodeGene val);
                    if (!success){
                        throw new InvalidOperationException($"New structure has node {id} that the highest parent does not have");
                    }
                    newNodesById[id] = val;
                }
            }

            NeuralNetwork child = new(this.pool, this.hasBias, newStructure, newNodesById);        

            Mutate(child);
            
            return child;
        }

        private void Mutate(NeuralNetwork child){
            foreach (var connection in child.structure){
                int roll = MutationRoll();
                if (roll <= WEIGHT_MUTATION_RATE){
                    // replace or mutate weight
                    if (roll <= REPLACEMENT_RATE){
                        connection.weight = random.Next(-2, 3) * random.NextDouble();
                    }
                    else{
                        connection.weight += random.Next(-1, 2) * MAX_WEIGHT_CHANGE;
                    }
                }
            }
            if (MutationRoll() <= FLIP_ACTIVITY_RATE){
                FlipRandomConnection(child);
            }
            if (MutationRoll() <= ADD_NODE_RATE){
                AddNode(child);
            }
            if (MutationRoll() <= ADD_CONNECTION_RATE){
                AddConnection(child);
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
            return random.Next(0, 101);
        }
    }
}