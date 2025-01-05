using System;
using System.Collections.Generic;

namespace NEAT{
    public class Species{
        public List<NeuralNetwork> memebers;
        public double avgFitness;
        public static readonly Random random = new();

        public Species(NeuralNetwork firstMember){
            memebers.Add(firstMember);
        }

        public double updateAvgFitness(){
            foreach (var member in memebers)
            {
                avgFitness += member.fitness;
            }
            avgFitness /= memebers.Count;
            return avgFitness;
        }

        public List<NeuralNetwork> CreateNewGeneration(double globalAvg){
            List<NeuralNetwork> newGen = new();
            int newSize = (int) (avgFitness / globalAvg) * memebers.Count;

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