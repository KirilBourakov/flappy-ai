using System;

namespace NEAT{
    public partial class NeuralNetwork{
        public static readonly int WEIGHT_MUTATION_RATE = 80;
        public static readonly int REPLACEMENT_RATE = 5;
        public static readonly double MAX_WEIGHT_CHANGE = 0.2;
        public static readonly int FLIP_ACTIVITY_RATE = 5;
        public static readonly int ADD_CONNECTION_RATE = 5;
        public static readonly int ADD_NODE_RATE = 3;
        private readonly Random random = new();

    }
}