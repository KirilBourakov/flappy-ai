using System;
using System.Collections.Generic;

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

            int baseLinePointer = 0;
            int targetPointer = 0;

            int baselineSize = 0;
            int targetSize = 0;
            
            while (!done){
                var currBaseline = (baseLinePointer >= 0 && baseLinePointer < target.structure.Count) ? target.structure[baseLinePointer] : null;
                while(currBaseline != null && !currBaseline.enabled){
                    baseLinePointer++;
                    currBaseline = (baseLinePointer >= 0 && baseLinePointer < target.structure.Count) ? target.structure[baseLinePointer] : null;
                }
                baselineSize++;
                var currTarget = (targetPointer >= 0 && targetPointer < baseline.structure.Count) ? baseline.structure[targetPointer] : null;
                while(currTarget != null && !currTarget.enabled){
                    targetPointer++;
                    currTarget = (targetPointer >= 0 && targetPointer < baseline.structure.Count) ? baseline.structure[targetPointer] : null;
                }
                targetSize++;

                if (currBaseline == null && currTarget == null){
                    done = true;
                }
                // excess
                else if (currTarget == null){
                    do
                    {
                        if (currBaseline.enabled){
                            targetSize++;
                            excess++;
                        }

                        baseLinePointer++;
                        currBaseline = (baseLinePointer >= 0 && baseLinePointer < target.structure.Count) ? target.structure[baseLinePointer] : null;
                    } while (currBaseline != null);
                    done = true;
                }
                else if (currBaseline == null){
                    do
                    {
                        if (currTarget.enabled){
                            excess++; 
                            baselineSize++;
                        } 
                        targetPointer++;
                        currTarget = (targetPointer >= 0 && targetPointer < baseline.structure.Count) ? baseline.structure[targetPointer] : null;
                    } while (currTarget != null);
                    done = true;
                }

                // innovations match
                else if (currBaseline.innovation == currTarget.innovation){
                    absDifference = Math.Abs(currBaseline.weight - currTarget.weight);
                    matchCount++;

                    baseLinePointer++;
                    targetPointer++;
                }

                // disjoint case
                else if (currBaseline.innovation > currTarget.innovation) {
                    disjoint++;
                    targetPointer++;
                }
                else if (currBaseline.innovation < currTarget.innovation) {
                    disjoint++;
                    baseLinePointer++;
                }          
            }

            double N = targetSize > baselineSize ? targetSize : baselineSize;
            double difference = c1*(excess/N) + c2*(disjoint/N) + c3*(absDifference/matchCount);
            return difference;
        }
    }
}