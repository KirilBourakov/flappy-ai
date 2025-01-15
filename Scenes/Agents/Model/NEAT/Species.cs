using System;
using System.Collections.Generic;
using Godot;

namespace NEAT{
    public class Species{
        //TODO: fix encapsulation
        public List<NeuralNetwork> memebers;
        public double avgAdjustedFitness;
        public double totalFitness;
        private double lastImprovement;
        private static readonly Random random = new();

        /// <summary>
        /// Create a species given an initial member
        /// </summary>
        /// <param name="firstMember"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Species(NeuralNetwork firstMember){
            if (firstMember == null)
            {
                throw new ArgumentNullException("Member cannot be null");
            }
            memebers = [firstMember];
        }
        /// <summary>
        /// Create a species given a list of it's memebers
        /// </summary>
        /// <param name="members"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Species(List<NeuralNetwork> members){
            this.memebers = members ?? throw new ArgumentNullException("Member cannot be null");
        }

        // todo: replace with property
        /// <summary>
        /// Update the current population fitness based on the fitness of it's members. Also keeps track of how long ago an imporovement occured from the last update.
        /// </summary>
        /// <returns></returns>
        public double updateFitnessFields(){
            double lastFit = avgAdjustedFitness;
            this.avgAdjustedFitness = 0;
            foreach (var member in memebers)
            {
                avgAdjustedFitness += member.adjustedFitness;
                totalFitness += member.fitness;
            }
            avgAdjustedFitness /= memebers.Count;
            if (lastFit <= this.avgAdjustedFitness){
                lastImprovement++;
            } else {
                lastImprovement = 0;
            }
            return avgAdjustedFitness;
        }

        public bool noImprovement(){
            return lastImprovement >= 15;
        }

        // TODO: handle species with only 1 member
        /// <summary>
        /// Creates a new generation of the species 
        /// </summary>
        /// <param name="globalAvg"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public List<NeuralNetwork> CreateNewGeneration(double globalAvg){
            if (memebers == null || memebers.Count == 0)
                throw new InvalidOperationException("Cannot create a new generation: memebers list is null or empty.");

            List<NeuralNetwork> newGen = new();
            int newSize = 1;
            if (globalAvg > 0) {
                newSize = (int) Math.Round((avgAdjustedFitness / globalAvg) * memebers.Count);
            } 

            for (int i = 0; i < memebers.Count; i++)
            {
                memebers[i].relativeFitness = memebers[i].fitness / totalFitness;
            }
            
            for (int i = 0; i < newSize; i++)
            {
                NeuralNetwork parent1 = getParent();
                NeuralNetwork parent2 = getParent();
                newGen.Add(parent1.Crossover(parent2));
            }

            return newGen;
        }

        /// <summary>
        /// Gets the next parent using a roulette wheel
        /// </summary>
        /// <returns></returns>
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