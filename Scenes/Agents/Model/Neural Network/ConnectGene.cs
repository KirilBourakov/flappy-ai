using System;

namespace NEAT{
    public class ConnectGene{
        public int inGene;
        public int outGene;
        public double weight;
        public bool enabled;
        public int innovation;

        private static int InnovationCount = 0;
        private static readonly Random random= new Random();

        /// <summary>
        /// Creates a ConnectGene given an input and output id
        /// </summary>
        public ConnectGene(int inGene, int outGene){
            this.inGene = inGene;
            this.outGene = outGene;
            this.enabled = true;
            this.weight = random.NextDouble() * random.Next(-2, 3);
            this.innovation = InnovationCount;
            InnovationCount++;
        }
        
        /// <summary>
        /// A pairing function to create a unique hash for a connect gene.
        /// </summary>
        /// <returns>A long representing the hash</returns>
        public long Hash(){
            var A = (ulong)(inGene >= 0 ? 2 * (long)inGene : -2 * (long)inGene - 1);
            var B = (ulong)(outGene >= 0 ? 2 * (long)outGene : -2 * (long)outGene - 1);
            var C = (long)((A >= B ? A * A + A + B : A + B * B) / 2);
            return inGene < 0 && outGene < 0 || inGene >= 0 && outGene >= 0 ? C : -C - 1;
        }

        /// <summary>
        /// A pairing function to create a unique hash for a connect gene.
        /// </summary>
        /// <param name="inp">Input node id.</param>
        /// <param name="outp">Output node id.</param>
        /// <returns>A long representing the hash</returns>
        public static long Hash(int inp, int outp){
            var A = (ulong)(inp >= 0 ? 2 * (long)inp : -2 * (long)inp - 1);
            var B = (ulong)(outp >= 0 ? 2 * (long)outp : -2 * (long)outp - 1);
            var C = (long)((A >= B ? A * A + A + B : A + B * B) / 2);
            return inp < 0 && outp < 0 || inp >= 0 && outp >= 0 ? C : -C - 1;
        }

        public ConnectGene Copy(){
            return (ConnectGene)this.MemberwiseClone();
        }
    }
}