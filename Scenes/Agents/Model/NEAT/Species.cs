using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NEAT{
    public class Species{
        //TODO: fix encapsulation
        public List<NeuralNetwork> memebers;
        public double totalFitness { get{ 
            double total = 0;
            foreach (NeuralNetwork n in memebers)
            {
                total += n.fitness;
            }
            return total;
        }}
        public double avgAdjustedFitness {get {
            double avg = 0;
            foreach (NeuralNetwork n in memebers){
                avg += n.getAdjustedFitness(memebers.Count);
            }
            return avg / memebers.Count;
        }}
        
        // TODO: punish species that do not improve
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
        public void NextGeneration(int newSize){
            if (memebers == null || memebers.Count == 0) throw new InvalidOperationException("Cannot create a new generation: memebers list is null or empty.");

            for (int i = 0; i < memebers.Count; i++){
                memebers[i].relativeFitness = memebers[i].fitness / totalFitness;
                GD.Print("relative " + i);
            }
            
            List<NeuralNetwork> newGen = [];
            for (int i = 0; i < newSize; i++){
                NeuralNetwork parent1 = getParent();
                NeuralNetwork parent2 = getParent();
                newGen.Add(parent1.Crossover(parent2));
                GD.Print($"new size {i} = {newSize}");
            }

            memebers = newGen;
        }

        /// <summary>
        /// Gets the next parent using a roulette wheel
        /// </summary>
        /// <returns></returns>
        private NeuralNetwork getParent(){
            if (totalFitness == 0 || memebers.All(m => m.relativeFitness == 0)) {
                return memebers[random.Next(memebers.Count)]; 
            }
            
            double target = random.NextDouble();
            double curr = 0;

            int i;
            for (i = 0; i < memebers.Count && curr < target; i++)
            {
                curr += memebers[i].relativeFitness;

                GD.Print($"curr {i} = {curr}");
            }
            GD.Print("Min: " + Math.Min(i, memebers.Count-1));
            return memebers[Math.Min(i, memebers.Count-1)];
        }
    }
}