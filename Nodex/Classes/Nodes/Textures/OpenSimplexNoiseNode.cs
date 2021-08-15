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

    struct NoiseSize
    {
        public NoiseSize(int width, int height, double scale)
        {
            Width = width;
            Height = height;
            Scale = scale;
        }

        public int Width;
        public int Height;
        public double Scale;
    }

    public partial class OpenSimplexNoiseNode : INode
    {
        public NodeControl nodeControl { get; set; }
        OpenSimplexNoiseCache noiseCache = new OpenSimplexNoiseCache();
        private Bitmap bitmap = null;
        private Bitmap unscaledBitmap = null;
        int width;
        int height;
        int tileSizeX = 256;
        int tileSizeY = 256;

        public OpenSimplexNoiseNode()
        {
            ComboBox comboBoxDimensions = new ComboBox() { Height = 24 };
            comboBoxDimensions.Items.Add(new ComboBoxItem() { Content = "2D" });
            comboBoxDimensions.Items.Add(new ComboBoxItem() { Content = "3D" });
            comboBoxDimensions.Items.Add(new ComboBoxItem() { Content = "4D" });
            comboBoxDimensions.SelectedIndex = 1;

            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                "OpenSimplex Noise",
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Vector, "Vector", NodeIO.NodeIOType.Input, true) },
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Output) },
                new NodeProperty[] { new NodeProperty(comboBoxDimensions, "Dimensions"), new NodeProperty(new IntegerUpDown(0, int.MaxValue) { Height = 24 }, "Seed"), new NodeProperty(new IntegerUpDown(128, 1024, 1, 4) { Height = 24 }, "Scale") },
                node.Calculate
                ), this.GetType())
            { Width = 175, Height = 70 };

            NoiseSize noiseSize = NormalizeWidthAndHeight(64);

            ParallelTextureCalculator parallelTextureCalculator = new ParallelTextureCalculator(new OpenSimplexNoise(0), tileSizeX, tileSizeY, noiseSize.Scale, new Vector4(0), new Size(noiseSize.Width, noiseSize.Height), Dimensions.Dimension.ThreeD);

            noiseCache = new OpenSimplexNoiseCache() { OpenSimplexNoise = new OpenSimplexNoise(0),
                ParallelTextureCalculator = parallelTextureCalculator,
                Seed = 0,
                Scale = 64,
                Dimension = Dimensions.Dimension.ThreeD,
                Vector = new Vector4(0),
                HighResCache = parallelTextureCalculator.Calculate(new Vector4(0), new Rectangle(new Point(0, 0), new Size(64, 64)))
            };
            //HighResCache = CalculateNoiseTexture(width * 4, height * 4, new Vector4(0), 0)};
        }

        NoiseSize NormalizeWidthAndHeight(int scale)
        {
            width = (int)App.Current.Properties["imageWidth"];
            height = (int)App.Current.Properties["imageHeight"];
            double scaleY;
            double scaleX;

            if (width > height)
            {
                scaleX = height;
                scaleY = (double)height / width * height;
            }
            else if (width < height)
            {
                scaleX = (double)width / height * width;
                scaleY = width;
            }
            else
            {
                scaleX = width;
                scaleY = height;
            }

            int tempX = (int)((double)scaleX / 512 * scale);
            int tempY = (int)((double)scaleY / 512 * scale);

            double textureScale = 4096 / Math.Min(scaleX, scaleY);

            return new NoiseSize(tempX, tempY, textureScale);
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            int seed = 0;
            int scale = 0;
            Dimensions.Dimension dimension = Dimensions.Dimension.ThreeD;
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

            if ((!(noiseCache.Seed != seed || noiseCache.Scale != scale || noiseCache.Dimension != dimension || noiseCache.Vector != vector || width != (int)App.Current.Properties["imageWidth"] || height != (int)App.Current.Properties["imageHeight"])) && bitmap != null)
            {
                return new object[] { bitmap };
            }

            NoiseSize noiseSize = NormalizeWidthAndHeight(scale);

            Debugger.Clear();
            Rectangle cropRect = new Rectangle((int)vector.X, (int)vector.Y, noiseSize.Width, noiseSize.Height);
            Debugger.AddValue(cropRect);

            if (noiseCache.Seed != seed)
                noiseCache.ParallelTextureCalculator = new ParallelTextureCalculator(new OpenSimplexNoise(seed), tileSizeX, tileSizeY, noiseSize.Scale, vector, new Size(noiseSize.Width, noiseSize.Height), dimension);
            if (noiseCache.Dimension != dimension)
                noiseCache.HighResCache = noiseCache.ParallelTextureCalculator.SetDimensionAndRecalculate(dimension);
            else
                noiseCache.HighResCache = noiseCache.ParallelTextureCalculator.Calculate(vector, cropRect);
            //noiseCache.HighResCache = CalculateNoiseTexture(width * 4, height * 4, new Vector4(0), seed);

            noiseCache.Dimension = dimension;
            noiseCache.Seed = seed;
            noiseCache.Scale = scale;

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
