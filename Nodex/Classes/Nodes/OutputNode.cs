using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nodex.Classes.Nodes
{
    public class OutputNode : INode
    {
        public NodeControl nodeControl { get; set; }

        public OutputNode()
        {
            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                    "Output",
                    new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Input) },
                    new NodeIO[0],
                    new NodeProperty[] { new NodeProperty(new IntegerUpDown(512, 16384, 0, 64) { Height = 24 }, "Output Width"), new NodeProperty(new IntegerUpDown(512, 16384, 0, 64) { Height = 24 }, "Output Height") },
                    node.Calculate
                    ), this.GetType())
            { Width = 120, Height = 70 };
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            object temp = inputs[0].value;

            if (temp == null)
                temp = 0;

            BitmapSource bitmapSource = null;

            switch (temp)
            {
                case Bitmap bitmap:
                    bitmapSource = bitmap.ConvertToBitmapSource();
                    break;
                case int integer:
                    int width = (int)App.Current.Properties["imageWidth"];
                    int height = (int)App.Current.Properties["imageHeight"];

                    Bitmap bmp = new Bitmap(width, height);
                    using (Graphics gfx = Graphics.FromImage(bmp))
                    using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(integer, integer, integer)))
                    {
                        gfx.FillRectangle(brush, 0, 0, width, height);
                    }
                    bitmapSource = bmp.ConvertToBitmapSource();
                    break;
                case NodeIO.NodeIOCategory.Undefined:
                    break;
            }

            if (bitmapSource != null)
                ((MainWindow)App.Current.MainWindow).imagePreview.Source = bitmapSource;
            else
                ((MainWindow)App.Current.MainWindow).imagePreview.Source = null;

            return new object[0];
        }
    }
}
