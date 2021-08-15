using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nodex.Classes.Nodes
{
    public class OutputNode : INode
    {
        public NodeControl nodeControl { get; set; }
        private IntegerUpDown widthIntegerUpDown;
        private IntegerUpDown heightIntegerUpDown;

        public OutputNode()
        {
            widthIntegerUpDown = new IntegerUpDown(512, 16384, 64, 64) { Height = 24 };
            heightIntegerUpDown = new IntegerUpDown(512, 16384, 64, 64) { Height = 24 };

            widthIntegerUpDown.ValueChanged += UpdateWidthAndHeight;
            heightIntegerUpDown.ValueChanged += UpdateWidthAndHeight;

            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                    "Output",
                    new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Input) },
                    new NodeIO[0],
                    new NodeProperty[] { new NodeProperty(widthIntegerUpDown, "Output Width"), new NodeProperty(heightIntegerUpDown, "Output Height") },
                    node.Calculate
                    ), this.GetType())
            { Width = 120, Height = 70 };
        }

        private void UpdateWidthAndHeight(object sender, EventArgs e)
        {
            App.Current.Properties["imageWidth"] = widthIntegerUpDown.Value;
            App.Current.Properties["imageHeight"] = heightIntegerUpDown.Value;
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            object temp;

            if (inputs == null)
                temp = 0;
            else
                temp = inputs[0].value;

            if (temp == null)
                temp = 0;

            BitmapSource bitmapSource = null;

            int width = 0;
            int height = 0;

            App.Current.Dispatcher.Invoke(() => {
                width = widthIntegerUpDown.Value;
                height = heightIntegerUpDown.Value;
            });

            try
            {
                switch (temp)
                {
                    case Bitmap bitmap:
                        //Resize Image if neccessary
                        //if (bitmap.Width != width || bitmap.Height != height)
                            bitmap = new Bitmap(bitmap, width, height);

                        App.Current.Dispatcher.Invoke(() => { bitmapSource = bitmap.ConvertToBitmapSource(); });
                        bitmap.Dispose();
                        break;
                    case int integer:
                        using (Bitmap bmp = new Bitmap(width, height))
                        {
                            using (Graphics gfx = Graphics.FromImage(bmp))
                            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(integer, integer, integer)))
                            {
                                gfx.FillRectangle(brush, 0, 0, width, height);
                            }
                            bitmapSource = bmp.ConvertToBitmapSource();
                        }
                        break;
                    case Vector3 vector:
                        break;
                    case NodeIO.NodeIOCategory.Undefined:
                        break;
                }
            }
            catch (Exception ex)
            {

            }

            App.Current.Dispatcher.Invoke(() => {
                if (bitmapSource != null)
                    ((MainWindow)App.Current.MainWindow).imagePreview.Source = bitmapSource;
                else
                    ((MainWindow)App.Current.MainWindow).imagePreview.Source = null;
            });

            return new object[0];
        }
    }
}
