using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nodex.Classes;

namespace Nodex.Resources.Controls
{
    /// <summary>
    /// Interaction logic for NodeOutput.xaml
    /// </summary>
    public partial class NodeOutputControl : UserControl
    {
        public NodeIO.NodeIOCategory NodeIOCategory { get; set; }
        public string Label { get; set; }
        public delegate void MouseLeftButtonDownHandler(object sender, MouseEventArgs e);
        public delegate void MouseLeftButtonUpHandler(object sender, MouseEventArgs e);
        public new event MouseLeftButtonDownHandler MouseLeftButtonDown;
        public new event MouseLeftButtonUpHandler MouseLeftButtonUp;

        public NodeOutputControl()
        {
            InitializeComponent();
        }

        public NodeOutputControl(NodeIO nodeIO)
        {
            InitializeComponent();

            Label = nodeIO.label;
            textLabel.Text = Label;

            NodeIOCategory = nodeIO.category;

            switch (NodeIOCategory)
            {
                case NodeIO.NodeIOCategory.Undefined:
                    ellipseOut.Fill = new LinearGradientBrush(Color.FromRgb(185, 185, 185), Color.FromRgb(64, 64, 64), 90);
                    break;
                case NodeIO.NodeIOCategory.Image:
                    ellipseOut.Fill = new LinearGradientBrush(Color.FromRgb(255, 235, 40), Color.FromRgb(100, 90, 0), 90);
                    break;
                case NodeIO.NodeIOCategory.Number:
                    ellipseOut.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(90, 90, 90), 90);
                    break;
                default:
                    break;
            }
        }

        private void ellipseOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseLeftButtonDown != null)
                MouseLeftButtonDown(sender, e);
        }

        private void ellipseOut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseLeftButtonUp != null)
                MouseLeftButtonUp(sender, e);
        }
    }
}
