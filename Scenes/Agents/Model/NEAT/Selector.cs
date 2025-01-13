using System;
using System.Collections.Generic;
using Godot;

namespace NEAT {
    public class Selector{

        private const double c1 = 1;
        private const double c2 = 1;
        private const double c3 = 0.4;

        public double threshold {get; private set;} = 4;
        private const double STEP = 0.3;
        private const int SPECIES_COUNT_TARGET = 5;

        private readonly Random random = new();

        // TODO: penalize species that do not evolve

        /// <summary>
        /// Creates an initial generation of a specific size
        /// </summary>
        /// <param name="genSize"></param>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <param name="useBias"></param>
        /// <param name="pool"></param>
        /// <returns></returns>
        public List<Species> CreateInitialGeneration(int genSize, int inputs, int outputs, bool useBias, out GenePool pool){
            pool = new GenePool();

            List<Species> newGen = [];
            List<NeuralNetwork> representatives = [];
            for (int i = 0; i < genSize; i++)
            {
                NeuralNetwork newNetwork = new(pool, inputs, outputs, useBias);
                for (int j = 0; j < representatives.Count; j++)
                {
                    if (Compare(representatives[j], newNetwork) < threshold){
                        newGen[j].memebers.Add(newNetwork);
                    } else {
                        representatives.Add(newNetwork);
                        newGen.Add(new Species(newNetwork));
                    }
                }
            }
            return newGen;
        }

        public List<Species> Speciate(List<Species> unSpeciated){
            List<NeuralNetwork> representatives = new();
            List<NeuralNetwork> nonRepresentatives = new();
            foreach (var singularSpecies in unSpeciated){
                NeuralNetwork rep = singularSpecies.memebers[random.Next(0, singularSpecies.memebers.Count)];
                representatives.Add(rep);
                foreach (var member in singularSpecies.memebers){
                    if (member != rep){
                        nonRepresentatives.Add(member);
                    }
                }
            }

            // TODO: does not handle species past initial ones given
            List<Species> speciated = new();
            foreach(var rep in representatives){
                Species newSpecies = new(rep);
                foreach(var nonRep in nonRepresentatives){
                    if (Compare(rep, nonRep) < threshold){
                        newSpecies.memebers.Add(nonRep);
                    }
                }
                speciated.Add(newSpecies);
            }

            if (speciated.Count > SPECIES_COUNT_TARGET){
                threshold += STEP;
            }
            else if (speciated.Count < SPECIES_COUNT_TARGET){
                threshold -= STEP;
            }
            return speciated;
        }

        /// <summary>
        /// Creates a new generation given a list of current species
        /// </summary>
        /// <param name="oldGeneration"></param>
        /// <returns></returns>
        public List<Species> CreateNewGeneration(List<Species> oldGen){
            oldGen = Speciate(oldGen);

            double fitAvg = 0;
            foreach (var singularSpecies in oldGen)
            {
                foreach (var member in singularSpecies.memebers){
                    member.adjustedFitness = member.fitness / singularSpecies.memebers.Count;
                }
                fitAvg += singularSpecies.updateFitnessFields();
            }
            fitAvg /= oldGen.Count;

            for (int i = 0; i<oldGen.Count; i++){
                oldGen[i].memebers = oldGen[i].CreateNewGeneration(fitAvg);
            }

            return oldGen;
        }

        /// <summary>
        /// Compares how similar a neuralNetwork is to a baseline
        /// </summary>
        /// <param name="baseline"></param>
        /// <param name="target"></param>
        /// <returns></returns>
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