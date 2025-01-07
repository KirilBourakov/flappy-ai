using System;
using System.Collections.Generic;
using Godot;

namespace NEAT{
    public class Species{
        public List<NeuralNetwork> memebers;
        public double avgFitness;
        public static readonly Random random = new();

        public Species(NeuralNetwork firstMember){
            if (firstMember == null)
            {
                throw new ArgumentNullException("Member cannot be null");
            }
            memebers = [firstMember];
        }
        public Species(List<NeuralNetwork> members){
            this.memebers = members ?? throw new ArgumentNullException("Member cannot be null");
        }

        public double updateAvgFitness(){
            this.avgFitness = 0;
            foreach (var member in memebers)
            {
                avgFitness += member.fitness;
            }
            avgFitness /= memebers.Count;
            return avgFitness;
        }

        public List<NeuralNetwork> CreateNewGeneration(double globalAvg){
            if (memebers == null || memebers.Count == 0)
                throw new InvalidOperationException("Cannot create a new generation: memebers list is null or empty.");

            List<NeuralNetwork> newGen = new();
            int newSize = 1;
            if (globalAvg > 0) {
                newSize = Math.Max(1, (int) (avgFitness / globalAvg) * memebers.Count);
            } 
            // todo: update how parents are chosen;
            for (int i = 0; i < newSize; i++)
            {
                NeuralNetwork parent1 = memebers[random.Next(0, memebers.Count)];
                NeuralNetwork parent2 = memebers[random.Next(0, memebers.Count)];
                newGen.Add(parent1.Crossover(parent2));
            }

            return newGen;
        }
    }
}