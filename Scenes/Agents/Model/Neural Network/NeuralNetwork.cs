
using System;
using System.Collections.Generic;
public class NeuralNetwork{
    private readonly Random random = new();
    public class Neuron{
        public float[] weights;
        public float value;
       
        private readonly Random random = new();
        public Neuron(int nueronsInNextLayer){
            weights = new float[nueronsInNextLayer];
            value = 0;

            for (int i = 0; i < weights.Length; i++)
            {
		        this.weights[i] = (float)(random.NextDouble() * 20 - 10);
            }
        }
        public Neuron(float value, float[] weights){
            this.value = value;
            this.weights = weights;
        }
        public void SetValue(float newval){
            this.value = newval;
        }
        public float GetValue(){
            return this.value;
        }

        public float GetWeight(int index){
            return this.weights[index];
        }
        public void SetWeight(int index, float newval){
            this.weights[index] = newval;
        }
        public void MutateWeight(int index, float mutation){
            this.weights[index] += mutation;
        }
        public float[] GetWeights(){
            return this.weights;
        }

        public Neuron DeepCopy(){
            return new Neuron(value, (float[])weights.Clone());
        }
    }
    
    public List<List<Neuron>> network = new List<List<Neuron>>();
    
    public NeuralNetwork(List<List<Neuron>> network){
        List<List<Neuron>> newNetwork = new();
        foreach (List<Neuron> listNodes in network)
        {
            List<Neuron> newListNodes = new();
            foreach (Neuron node in listNodes)
            {
                newListNodes.Add(node.DeepCopy());
            }
            newNetwork.Add(newListNodes);
        }
        this.network = newNetwork;
    }
    public NeuralNetwork(int[] layerNeuronCounts){
        for (int i = 0; i < layerNeuronCounts.Length; i++)
        {
            int layerNeuronCount = layerNeuronCounts[i];
            int nueronsInNextLayer = i+1 < layerNeuronCounts.Length ? layerNeuronCounts[i+1] : 0;

            List<Neuron> layer = new List<Neuron>();
            for (int j = 0; j < layerNeuronCount; j++)
            {
                layer.Add(new Neuron(nueronsInNextLayer));
            }
            network.Add(layer);
        }
    }

    public int[] Evaluate(float[] inputs){
        for(int i = 0; i<inputs.Length; i++){
            network[0][i].SetValue(inputs[i]);
        }

        for (int i = 1; i < network.Count; i++){
            for (int j = 0; j < network[i].Count; j++){
                float new_val = 0;
                for (int k = 0; k < network[i-1].Count; k++){
                    new_val += network[i-1][k].GetValue() * network[i-1][k].GetWeight(j);
                }
                
                network[i][j].SetValue(new_val);
            }
        }

        int[] final = new int[network[^1].Count];
        for (int i = 0; i < network[^1].Count; i++)
        {
            final[i] = StepFunction(network[^1][i].GetValue());
        }

        return final;
    }

    private static int StepFunction(float inp){
		return inp > 0 ? 1 : 0;
	}

    public NeuralNetwork Reproduce(NeuralNetwork mate){
        NeuralNetwork child = new(this.network);

        for (int i = 0; i < network.Count; i++){
            for (int j = 0; j < network[i].Count; j++){
                for (int k = 0; k < network[i][j].GetWeights().Length; k++){
                    if(random.Next(2) == 1){
                        child.network[i][j].SetWeight(k, mate.network[i][j].GetWeight(k)); 
                    }

                    if(random.Next(100) == 0){
                        child.network[i][j].MutateWeight(k, GetGaussianMutation());
                    }
                }
            }
        }

        return child;
    }

    public float GetGaussianMutation(){
		double u1 = random.NextDouble();
		double u2 = random.NextDouble();
		return (float) (0.1 * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2));
	}
}