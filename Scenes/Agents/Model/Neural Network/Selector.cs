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
        public List<NeuralNetwork> CreateNewGeneration(List<NeuralNetwork> oldGeneration){
            List<NeuralNetwork> newGeneration = new();
            
            List<Species> species = new();
            foreach (var member in oldGeneration)
            {
                bool matchExists = false;
                foreach (var singularSpecies in species){
                    if (Compare(singularSpecies.memebers[0], member) > threshold){
                        matchExists = true;
                        singularSpecies.memebers.Add(member);
                        break;
                    }
                }
                if (!matchExists){
                    species.Add(new Species(member));
                }
            }
            
            // adjust fitness
            double fitAvg = 0;
            foreach (var singularSpecies in species)
            {
                foreach (var member in singularSpecies.memebers){
                    member.adjustedFitness = member.fitness / singularSpecies.memebers.Count;
                }
                fitAvg += singularSpecies.updateFitnessFields();
            }
            fitAvg /= species.Count;

            foreach (var singularSpecies in species)
            {
                newGeneration.AddRange(singularSpecies.CreateNewGeneration(fitAvg));
            }

            return newGeneration;
        }

        public List<Species> CreateNewGeneration(List<Species> oldGen){
            List<NeuralNetwork> representatives = new();
            List<NeuralNetwork> nonRepresentatives = new();
            foreach (var singularSpecies in oldGen){
                NeuralNetwork rep = singularSpecies.memebers[random.Next(0, singularSpecies.memebers.Count)];
                representatives.Add(rep);
                foreach (var member in singularSpecies.memebers){
                    if (member != rep){
                        nonRepresentatives.Add(member);
                    }
                }
            }

            List<Species> speciatedOldGen = new();
            foreach(var rep in representatives){
                Species newSpecies = new(rep);
                foreach(var nonRep in nonRepresentatives){
                    if (Compare(rep, nonRep) < 4){
                        newSpecies.memebers.Add(nonRep);
                    }
                }
                speciatedOldGen.Add(newSpecies);
            }

            if (speciatedOldGen.Count > SPECIES_COUNT_TARGET){
                threshold += STEP;
            }
            else if (speciatedOldGen.Count < SPECIES_COUNT_TARGET){
                threshold -= STEP;
            }

            double fitAvg = 0;
            foreach (var singularSpecies in speciatedOldGen)
            {
                foreach (var member in singularSpecies.memebers){
                    member.adjustedFitness = member.fitness / singularSpecies.memebers.Count;
                }
                fitAvg += singularSpecies.updateFitnessFields();
            }
            fitAvg /= speciatedOldGen.Count;

            for (int i = 0; i<speciatedOldGen.Count; i++){
                speciatedOldGen[i].memebers = speciatedOldGen[i].CreateNewGeneration(fitAvg);
            }

            return speciatedOldGen;
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