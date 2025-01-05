using System;
using System.Collections.Generic;

namespace NEAT {
    public class Selector(){

        private const double c1 = 1;
        private const double c2 = 1;
        private const double c3 = 0.4;

        // penalize species that do not evolve
        public List<NeuralNetwork> CreateNewGeneration(List<NeuralNetwork> newGeneration){
            // speciation

            //


            return new List<NeuralNetwork>();
        }

        private double Compare(NeuralNetwork baseline, NeuralNetwork target){
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

                // excess
                if (currTarget == null){
                    do
                    {
                        if (currTarget.enabled){
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
                        if (currBaseline.enabled){
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