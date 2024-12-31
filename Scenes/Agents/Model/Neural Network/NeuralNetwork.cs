using System;
using System.Collections.Generic;
using System.Xml;

namespace NEAT{
    public class NeuralNetwork{
        public GenePool pool;
        
        // sort topologically, then by id among inputs
        public List<ConnectGene> structure = new List<ConnectGene>();
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

        public double[] Evaluate(double[] inpt){

            for (int i = 0; i < inpt.Length; i++)
            {
                pool.SafeGetNode(i, NodeGene.Type.INPUT).Value = inpt[i];
            }

            if (hasBias){
                pool.SafeGetNode(inpt.Length, NodeGene.Type.INPUT).Value = 1;
            }


            var output = this.pool.getGeneByType(NodeGene.Type.OUTPUT);
            double[] result = new double[output.Count];
            for (int i = 0; i < output.Count; i++){
                result[i] = output[i].Value;
            }

            return result;
        }
    }
}