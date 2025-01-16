using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;

namespace NEAT {
    public class Population{

        private const double c1 = 1;
        private const double c2 = 1;
        private const double c3 = 0.4;

        public double threshold {get; private set;} = 4;
        private const double STEP = 0.3;
        private const int SPECIES_COUNT_TARGET = 5;

        private readonly Random random = new();

        public List<Species> population {get;}

        public int totalMembers {get {
            int i = 0;
            foreach (var species in population)
            {
                i += species.memebers.Count;   
            }
            return i;
        }}

        /// <summary>
        /// Creates an initial generation of a specific size
        /// </summary>
        /// <param name="genSize"></param>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <param name="useBias"></param>
        /// <param name="pool"></param>
        /// <returns></returns>
        public Population(int genSize, int inputs, int outputs, bool useBias, out GenePool pool){
            pool = new GenePool();

            List<Species> newGen = [];
            List<NeuralNetwork> representatives = [];
            for (int i = 0; i < genSize; i++)
            {
                NeuralNetwork newNetwork = new(pool, inputs, outputs, useBias);
                bool added = false;
                for (int j = 0; j < representatives.Count && !added; j++)
                {
                    if (Compare(representatives[j], newNetwork) < threshold && !added){
                        newGen[j].memebers.Add(newNetwork);
                        added = true;
                    }
                }
                if (!added){
                    representatives.Add(newNetwork);
                    newGen.Add(new Species(newNetwork));
                }
            }

            population = newGen;
        }

        /// <summary>
        /// ReSpeciates the species
        /// </summary>
        /// <param name="unSpeciated"></param>
        /// <returns></returns>
        private void ReSpeciate(){
            // get representatives, and unlabel labled members
            List<NeuralNetwork> representatives = new();
            List<NeuralNetwork> nonRepresentatives = new();
            foreach (var singularSpecies in population){
                NeuralNetwork rep = singularSpecies.memebers[random.Next(0, singularSpecies.memebers.Count)];
                representatives.Add(rep);
                foreach (var member in singularSpecies.memebers){
                    if (member != rep){
                        nonRepresentatives.Add(member);
                    }
                }
            }

            // clear the members of the species
            foreach (var singleSpecies in population)
            {
                singleSpecies.memebers = [];
            }

            // get the members for each representative
            bool[] placed = new bool[nonRepresentatives.Count];
            for (int i = 0; i < representatives.Count; i++){
                List<NeuralNetwork> newMembers = [representatives[i]];
                for (int j = 0; j < nonRepresentatives.Count; j++){
                    if (!placed[j] && Compare(representatives[i], nonRepresentatives[j]) < threshold){
                        placed[j] = true;
                        newMembers.Add(nonRepresentatives[j]);
                    }   
                }
                population[i].memebers = newMembers;
            }

            // get the members for species that did not have representatives 
            for (int i = 0; i < placed.Length; i++){
                if (!placed[i]){
                    Species newSpecies = new(nonRepresentatives[i]);
                    for (int j = i; j < placed.Length; j++){
                        if (!placed[j]){
                            newSpecies.memebers.Add(nonRepresentatives[j]);
                            placed[j] = true;
                        }
                    }
                    population.Add(newSpecies);
                }
            }

            // pruge empty species and those that have not improved
            for (int i = population.Count - 1; i >= 0; i--)
            {
                if (population[i].memebers.Count == 0 || population[i].noImprovement()){
                    population.RemoveAt(i);
                }
            }

            if (population.Count > SPECIES_COUNT_TARGET){
                threshold += STEP;
            }
            else if (population.Count < SPECIES_COUNT_TARGET){
                threshold -= STEP;
            }
        }

        /// <summary>
        /// Creates a new generation given a list of current species
        /// </summary>
        /// <param name="oldGeneration"></param>
        /// <returns></returns>
        public void CreateNewGeneration(){

            ReSpeciate();

            double fitAvg = 0;
            foreach (var singularSpecies in population)
            {
                fitAvg += singularSpecies.avgAdjustedFitness;
            }
            fitAvg /= population.Count;
            

            for (int i = population.Count - 1; i >= 0; i--){
                population[i].memebers = population[i].CreateNewGeneration(fitAvg, out bool extinct);
                if (extinct){
                    population.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Compares how similar a neuralNetwork is to a baseline
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// 
        //TODO: rewrite
        public double Compare(NeuralNetwork baseline, NeuralNetwork target){
            baseline.structure.Sort((x, y) => x.innovation.CompareTo(y.innovation));
            target.structure.Sort((x, y) => x.innovation.CompareTo(y.innovation));
            
            bool done = false;

            int excess = 0;
            int disjoint = 0;

            double absDifference = 0;
            int matchCount = 0;

            int baseLinePointer = -1;
            int targetPointer = 0;

            int baselineSize = -1;
            int targetSize = 0;

            // TODO: handling disjoint and excess incorreclty (See: [1,2,4,5] & [1,2,3,4])
            while (!done){
                ConnectGene currBaseline = baseline.NextGene(ref baseLinePointer, ref baselineSize);
                ConnectGene currTarget = target.NextGene(ref targetPointer, ref targetSize);

                if (currBaseline == null && currTarget == null){
                    done = true;
                }
                // excess
                else if (currTarget == null || currBaseline == null){
                    excess++;
                }
                // innovations match
                else if (currBaseline.innovation == currTarget.innovation){
                    absDifference = Math.Abs(currBaseline.weight - currTarget.weight);
                    matchCount++;
                } else {
                    // disjoint case
                    while (currTarget != null && currBaseline.innovation > currTarget.innovation){
                        disjoint++;
                        currTarget = target.NextGene(ref targetPointer, ref targetSize);
                    }
                    while (currBaseline != null && currTarget != null && currBaseline.innovation < currTarget.innovation){
                        disjoint++;
                        currBaseline = baseline.NextGene(ref baseLinePointer, ref baselineSize);
                    } 
                }       
            }

            double makeSafe(double inp){
                return Double.IsFinite(inp) ? inp : 0;
            }

            double N = targetSize > baselineSize ? targetSize : baselineSize;
            if (N < 20) N=1;
            double difference = makeSafe(c1*(excess/N)) + makeSafe(c2*(disjoint/N)) + makeSafe(c3*(absDifference/matchCount));
            return difference;
        }
    }
}