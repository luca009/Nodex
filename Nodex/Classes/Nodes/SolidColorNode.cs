using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Nodex.Classes.Nodes
{
    public partial class SolidColorNode : INode
    {
        public NodeControl nodeControl { get; set; }
        byte R = 255;
        byte G = 255;
        byte B = 255;

        public SolidColorNode()
        {
            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                "Solid Color",
                new NodeIO[0],
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Output) },
                new NodeProperty[] { new NodeProperty(new IntegerUpDown(255, 255, 0, 3) { Height = 24 }, "R"), new NodeProperty(new IntegerUpDown(255, 255, 0, 3) { Height = 24 }, "G"), new NodeProperty(new IntegerUpDown(255, 255, 0, 3) { Height = 24 }, "B") },
                node.Calculate
                ), this.GetType())
            { Width = 120, Height = 70 };
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            R = (byte)((IntegerUpDown)properties[0].propertyElement).Value;
            G = (byte)((IntegerUpDown)properties[1].propertyElement).Value;
            B = (byte)((IntegerUpDown)properties[2].propertyElement).Value;

            int width = (int)App.Current.Properties["imageWidth"];
            int height = (int)App.Current.Properties["imageHeight"];

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(R, G, B)))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
            }

            return new object[] { bmp };
        }
    }
}
