using System;
using System.Collections.Generic;
using System.Xml;

namespace NEAT{
    public class NeuralNetwork{
        public GenePool pool;
        
        public List<ConnectGene> structure = new();
        public bool hasBias;

        public NeuralNetwork(GenePool pool, int inputs, int outputs, bool useBias){
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
            // prepare nodes for insertion
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
    }
}