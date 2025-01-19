
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NEAT{
    public partial class NeuralNetwork{
        private class PositionedNode{
            private NodeGene node;
            private Vector2? _position = null;
            public Vector2? position {
                get => _position;
                set{
                    if (value == null) throw new ArgumentNullException(nameof(value), "Position cannot be null.");
                    if (value.Value.X <= 0 || value.Value.Y <= 0) throw new InvalidDataException($"Cannot position a node at ({value.Value.X}, {value.Value.Y})");
                    _position = value;
                }
            }

            public PositionedNode(NodeGene node){
                this.node = node;
            }
        }

        private const int nodeSize = 30;
        private const int padding = 10;
        private const int distanceBetweenLayers = 55;

        public void visualize(string outputPath)
        {
            List<List<PositionedNode>> nodes = [];
            foreach (NodeGene nodeGene in nodeById.Values)
            {
                int listIndex = nodeGene.Layer - 1;

                if (nodes.Count <= listIndex){
                    for (int i = nodes.Count; i <= listIndex; i++)
                    {
                        nodes.Add([]);
                    }
                }
                nodes[listIndex].Add(new PositionedNode(nodeGene));
            }
            updatedPos(nodes, out int networkWidth, out int networkDepth);
            
            // Define the image size
            int width = distanceBetweenLayers * (networkDepth+2);
            int height = nodeSize * (networkWidth+4);
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Black);
                Brush brush = new SolidBrush(Color.Blue);
                foreach (var layer in nodes)
                {
                    foreach (var node in layer)
                    {
                        if (node.position == null) throw new ArgumentNullException("Node position not updated.");
                        Rectangle rect = new Rectangle((int)node.position.Value.X, (int)node.position.Value.Y, nodeSize, nodeSize);
                        graphics.FillEllipse(brush, rect);

                    }
                }
                brush.Dispose();
            }

            bitmap.Save(outputPath, ImageFormat.Jpeg);
            bitmap.Dispose();
            Godot.GD.Print($"Image saved as {outputPath}");
        }

        private void updatedPos(List<List<PositionedNode>> nodes, out int networkWidth, out int networkDepth){
            int maxLength = 0;
            for (int i = 0; i < nodes.Count; i++){
                if (nodes[i].Count > maxLength) maxLength = nodes[i].Count;

                for (int j = 0; j < nodes[i].Count; j++)
                {
                    nodes[i][j].position = GetPosition(j, i);
                }
            }
            networkWidth = maxLength;
            networkDepth = nodes.Count;
        }

        private Vector2 GetPosition(int numInLayer, int layer){
            return  new Vector2(
                (numInLayer+1)*(nodeSize+2*padding), 
                (layer+1)*distanceBetweenLayers
            );
        }
    }

    
}
