using Nodex.Classes.TextureGenerators;
using Nodex.Classes.Threading;
using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nodex.Classes.Nodes.Textures
{
    struct OpenSimplexNoiseCache
    {
        public OpenSimplexNoise OpenSimplexNoise;
        public ParallelTextureCalculator ParallelTextureCalculator;
        public long Seed;
        public int Scale;
        public Dimensions.Dimension Dimension;
        public Vector4 Vector;
        public Bitmap HighResCache;
    }

    public partial class OpenSimplexNoiseNode : INode
    {
        public NodeControl nodeControl { get; set; }
        OpenSimplexNoiseCache noiseCache = new OpenSimplexNoiseCache();
        private Bitmap bitmap = null;
        private Bitmap unscaledBitmap = null;
        int width;
        int height;

        public OpenSimplexNoiseNode()
        {
            ComboBox comboBoxDimensions = new ComboBox() { Height = 24, SelectedIndex = 0 };
            comboBoxDimensions.Items.Add(new ComboBoxItem() { Content = "2D" });
            comboBoxDimensions.Items.Add(new ComboBoxItem() { Content = "3D" });
            comboBoxDimensions.Items.Add(new ComboBoxItem() { Content = "4D" });

            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                "OpenSimplex Noise",
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Vector, "Vector", NodeIO.NodeIOType.Input, true) },
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Output) },
                new NodeProperty[] { new NodeProperty(comboBoxDimensions, "Dimensions (WIP)"), new NodeProperty(new IntegerUpDown(0, int.MaxValue) { Height = 24 }, "Seed"), new NodeProperty(new IntegerUpDown(128, 1024, 1, 4) { Height = 24 }, "Scale") },
                node.Calculate
                ), this.GetType())
            { Width = 175, Height = 70 };

            width = (int)App.Current.Properties["imageWidth"];
            height = (int)App.Current.Properties["imageHeight"];

            ParallelTextureCalculator parallelTextureCalculator = new ParallelTextureCalculator(new OpenSimplexNoise(0), width, height, 8, new Vector4(0), new Size(128, 128));

            noiseCache = new OpenSimplexNoiseCache() { OpenSimplexNoise = new OpenSimplexNoise(0),
                ParallelTextureCalculator = parallelTextureCalculator,
                Seed = 0,
                Scale = 64,
                Dimension = Dimensions.Dimension.TwoD,
                Vector = new Vector4(0),
                HighResCache = parallelTextureCalculator.Calculate(new Vector4(0), new Rectangle(new Point(0, 0), new Size(64, 64)))
            };
            //HighResCache = CalculateNoiseTexture(width * 4, height * 4, new Vector4(0), 0)};
        }

        Bitmap CalculateNoiseTexture(int resolutionX, int resolutionY, Vector4 vector, long seed, OpenSimplexNoise openSimplexNoise = null)
        {
            if (openSimplexNoise == null)
                openSimplexNoise = new OpenSimplexNoise(seed);

            Bitmap bitmap = new Bitmap(resolutionX, resolutionY);
            for (int x = 0; x < resolutionX; x++)
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    int pixelBrightness = (int)((openSimplexNoise.Evaluate((x + vector.X) / 8, (y + vector.Y) / 8, vector.Z, vector.W) + 1) * 127.5);
                    bitmap.SetPixel(x, y, Color.FromArgb(pixelBrightness, pixelBrightness, pixelBrightness));
                }
            }

            return bitmap;
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            if (bitmap != null && (width != (int)App.Current.Properties["imageWidth"] || height != (int)App.Current.Properties["imageHeight"]))
            {
                width = (int)App.Current.Properties["imageWidth"];
                height = (int)App.Current.Properties["imageHeight"];
                bitmap = new Bitmap(unscaledBitmap, width, height);
            }

            int seed = 0;
            int scale = 0;
            Dimensions.Dimension dimension = Dimensions.Dimension.TwoD;
            Vector4 vector = new Vector4(0);

            App.Current.Dispatcher.Invoke(() =>
            {
                seed = ((IntegerUpDown)properties[1].propertyElement).Value;
                scale = ((IntegerUpDown)properties[2].propertyElement).Value;
                dimension = (Dimensions.Dimension)((ComboBox)properties[0].propertyElement).SelectedIndex;
                if (inputs[0].connectedNodeIOs != null && inputs[0].connectedNodeIOs[0].value != null)
                    vector = (Vector4)inputs[0].connectedNodeIOs[0].value;
                Debugger.AddValue("vector: " + vector.ToString());
            });

            if (!(noiseCache.Seed != seed || noiseCache.Scale != scale || noiseCache.Dimension != dimension || noiseCache.Vector != vector) && bitmap != null)
            {
                return new object[] { bitmap };
            }

            width = (int)App.Current.Properties["imageWidth"];
            height = (int)App.Current.Properties["imageHeight"];

            Rectangle cropRect = new Rectangle((int)vector.X, (int)vector.Y, scale, scale);

            if (noiseCache.Seed != seed)
                noiseCache.ParallelTextureCalculator = new ParallelTextureCalculator(new OpenSimplexNoise(seed), width, height, 8, vector, new Size(128, 128));
            //noiseCache.Vector != vector
            noiseCache.HighResCache = noiseCache.ParallelTextureCalculator.Calculate(vector, cropRect);
                //noiseCache.HighResCache = CalculateNoiseTexture(width * 4, height * 4, new Vector4(0), seed);

            noiseCache.Dimension = dimension;
            noiseCache.Seed = seed;
            noiseCache.Scale = scale;
            double scaleSquared = scale ^ 2;

            //Bitmap sourceBitmap = noiseCache.HighResCache;
            //Bitmap targetBitmap = new Bitmap(cropRect.Width, cropRect.Height);

            //using (Graphics gfx = Graphics.FromImage(targetBitmap))
            //{
            //    gfx.DrawImageUnscaledAndClipped(sourceBitmap, new Rectangle(0, 0, targetBitmap.Width, targetBitmap.Height));
            //}

            Bitmap returnBitmap = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics gfx = Graphics.FromImage(returnBitmap))
            {
                gfx.DrawImage(noiseCache.HighResCache, -cropRect.X, -cropRect.Y);
            }

            bitmap = returnBitmap;

            return new object[] { returnBitmap };
        }
    }
}
