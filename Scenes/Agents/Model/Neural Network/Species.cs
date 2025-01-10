using System;
using System.Collections.Generic;
using Godot;

namespace NEAT{
    public class Species{
        //TODO: fix encapsulation
        public List<NeuralNetwork> memebers;
        public double avgAdjustedFitness;
        public double totalFitness;
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

        // todo: replace with property
        public double updateFitnessFields(){
            this.avgAdjustedFitness = 0;
            foreach (var member in memebers)
            {
                avgAdjustedFitness += member.adjustedFitness;
                totalFitness += member.fitness;
            }
            avgAdjustedFitness /= memebers.Count;
            return avgAdjustedFitness;
        }

        // TODO: handle species with only 1 member
        public List<NeuralNetwork> CreateNewGeneration(double globalAvg){
            if (memebers == null || memebers.Count == 0)
                throw new InvalidOperationException("Cannot create a new generation: memebers list is null or empty.");

            List<NeuralNetwork> newGen = new();
            int newSize = 1;
            if (globalAvg > 0) {
                newSize = Math.Max(1, (int) (avgAdjustedFitness / globalAvg) * memebers.Count);
            } 

            for (int i = 0; i < memebers.Count; i++)
            {
                memebers[i].relativeFitness = memebers[i].fitness / totalFitness;
            }
            
            for (int i = 0; i < newSize; i++)
            {
                double target = random.NextDouble();

                NeuralNetwork parent1 = getParent();
                NeuralNetwork parent2;
                do {
                    parent2 = getParent();
                } while (parent1 == parent2 && memebers.Count != 1);
                newGen.Add(parent1.Crossover(parent2));
            }

            return newGen;
        }

        private NeuralNetwork getParent(){
            double target = random.NextDouble();
            double curr = 0;

            int i;
            for (i = 0; i < memebers.Count && curr < target; i++)
            {
                curr += memebers[i].relativeFitness;
            }

            return memebers[Math.Min(i, memebers.Count-1)];
        }
    }
}