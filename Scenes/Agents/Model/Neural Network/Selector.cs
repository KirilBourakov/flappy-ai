using System;
using System.Collections.Generic;
using Godot;

namespace NEAT {
    public class Selector{

        private const double c1 = 1;
        private const double c2 = 1;
        private const double c3 = 0.4;

        public const double threshold = 4;

        // TODO: penalize species that do not evolve
        public List<NeuralNetwork> CreateNewGeneration(List<NeuralNetwork> oldGeneration){
            List<NeuralNetwork> newGeneration = new();

            // speciation
            List<Species> species = new();
            int currentSpecies = 1;
            bool done = false;

            NeuralNetwork baseLine;
            int i = 0;
            while (!done){
                NeuralNetwork examined = (i >= 0 && i < oldGeneration.Count) ? oldGeneration[i] : null;
                if (examined == null){
                    done = true;
                }
                else if (species.Count < currentSpecies){
                    baseLine = examined;
                    Species newSpeices = new(baseLine);
                    species.Add(newSpeices);
                }
                else {
                   species[currentSpecies-1].memebers.Add(examined);
                }
                i++;
            }

            // adjust fitness
            double fitAvg = 0;
            foreach (var singularSpecies in species)
            {
                foreach (var member in singularSpecies.memebers){
                    member.fitness /= singularSpecies.memebers.Count;
                }
                fitAvg += singularSpecies.updateAvgFitness();
            }
            fitAvg /= species.Count;

            foreach (var singularSpecies in species)
            {
                newGeneration.AddRange(singularSpecies.CreateNewGeneration(fitAvg));
            }

            return newGeneration;
        }

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
            double N = targetSize > baselineSize ? targetSize : baselineSize;
            double difference = c1*(excess/N) + c2*(disjoint/N) + c3*(absDifference/matchCount);
            return difference;
        }
    }
}