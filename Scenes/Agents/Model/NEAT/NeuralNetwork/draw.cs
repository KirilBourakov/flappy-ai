
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
            public Vector2 leftHandCorner {
                get => new Vector2(_position.Value.X-(nodeSize/2), _position.Value.Y-(nodeSize/2));
            }

            public PositionedNode(NodeGene node){
                this.node = node;
            }
        }

        private const int nodeSize = 30;
       
        private const int distanceBetweenLayers = 55;


        private const int MIN_CELL_SIZE = 30;
        private const int PADDING = 10;
        public void Visualize(string outputPath){
            List<List<PositionedNode>> nodes = [];
            Dictionary<int, PositionedNode> positionNodeById = [];
            foreach (NodeGene nodeGene in nodeById.Values)
            {
                int listIndex = nodeGene.Layer - 1;

                if (nodes.Count <= listIndex){
                    for (int i = nodes.Count; i <= listIndex; i++)
                    {
                        nodes.Add([]);
                    }
                }
                PositionedNode p = new PositionedNode(nodeGene);
                positionNodeById[nodeGene.nodeId] = p;
                nodes[listIndex].Add(p);
            }

            int largestLayer = 0;
            foreach (var layer in nodes)
            {
                if (layer.Count > largestLayer){
                    largestLayer = layer.Count;
                }
            }
            int networkLength = nodes.Count;

            int bitmapHight = largestLayer * MIN_CELL_SIZE;
            int bitmapWidth = networkLength * 2 * MIN_CELL_SIZE;
            // update node position
            updatePos(nodes, bitmapHight, bitmapWidth);

            // draw
            Bitmap bitmap = new Bitmap(bitmapWidth*3, bitmapHight*3);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Black);
                Brush blue = new SolidBrush(Color.Blue);
                Brush red = new SolidBrush(Color.Red);
                Pen bluePen = new (blue, 5);
                Pen redPen = new (red, 5);
                foreach (var layer in nodes)
                {
                    foreach (var connection in structure)
                    {
                        var inNode = positionNodeById[connection.inGene];
                        var outNode = positionNodeById[connection.outGene];
                        graphics.DrawLine(connection.enabled ? bluePen : redPen, 
                            new Point((int)inNode.position.Value.X, (int)inNode.position.Value.Y),
                            new Point((int)outNode.position.Value.X, (int)outNode.position.Value.Y)
                        );
                    }

                    foreach (var node in layer)
                    {
                        if (node.position == null) throw new ArgumentNullException("Node position not updated.");
                        Rectangle rect = new Rectangle((int)node.leftHandCorner.X, (int)node.leftHandCorner.Y, nodeSize, nodeSize);
                        graphics.FillEllipse(blue, rect);

                    }
                }

                red.Dispose();
                blue.Dispose();
            }

            bitmap.Save(outputPath, ImageFormat.Jpeg);
            bitmap.Dispose();
            Godot.GD.Print($"Image saved as {outputPath}");
        }


        private void updatePos(List<List<PositionedNode>> nodes, int bitmapHight, int bitmapWidth){
            for (int i = 0; i < nodes.Count; i++)
            {
                List<PositionedNode> layer = nodes[i];
                int cellSize = bitmapHight / layer.Count;
                for (int j = 0; j < layer.Count; j++)
                {
                    int xPos = (2*i+1)*MIN_CELL_SIZE + (MIN_CELL_SIZE/2);
                    int yPos = j * (cellSize+PADDING) + (cellSize/2);
                    layer[j].position = new Vector2(xPos, yPos);
                }
            }
        }
    }    
}
