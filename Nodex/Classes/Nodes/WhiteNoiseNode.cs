using Nodex.Resources.Controls;
using System;
using System.Drawing;

namespace Nodex.Classes.Nodes
{
    public partial class WhiteNoiseNode : INode
    {
        public NodeControl nodeControl { get; set; }
        private Bitmap bitmap = null;
        private Bitmap unscaledBitmap = null;
        int seed = 0;
        Int16 scale = 64;
        int width;
        int height;

        public WhiteNoiseNode()
        {
            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                "White Noise",
                new NodeIO[0],
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Output) },
                new NodeProperty[] { new NodeProperty(new IntegerUpDown(64, 512, 4, 4) { Height = 24 }, "Scale"), new NodeProperty(new IntegerUpDown(0, int.MaxValue, 0, 1) { Height = 24 }, "Seed") },
                node.Calculate
                ), this.GetType())
            { Width = 120, Height = 70 };
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            if (bitmap != null && (width != (int)App.Current.Properties["imageWidth"] || height != (int)App.Current.Properties["imageHeight"]))
            {
                width = (int)App.Current.Properties["imageWidth"];
                height = (int)App.Current.Properties["imageHeight"];
                bitmap = new Bitmap(unscaledBitmap, width, height);
            }

            if (bitmap != null && scale == (Int16)((IntegerUpDown)properties[0].propertyElement).Value && seed == ((IntegerUpDown)properties[1].propertyElement).Value)
                return new object[] { bitmap };

            width = (int)App.Current.Properties["imageWidth"];
            height = (int)App.Current.Properties["imageHeight"];
            scale = (Int16)((IntegerUpDown)properties[0].propertyElement).Value;
            seed = ((IntegerUpDown)properties[1].propertyElement).Value;

            Bitmap tempBitmap = new Bitmap(scale, scale);
            Random random = new Random(seed);
            for (int x = 0; x < scale; x++)
            {
                for (int y = 0; y < scale; y++)
                {
                    int tempRandom = random.Next(0, 256);
                    tempBitmap.SetPixel(x, y, Color.FromArgb(tempRandom, tempRandom, tempRandom));
                }
            }

            unscaledBitmap = tempBitmap;

            Bitmap returnBitmap = new Bitmap(tempBitmap, width, height);
            bitmap = returnBitmap;

            return new object[] { returnBitmap };
        }
    }
}
