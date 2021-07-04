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
        public NodeOutput.NodeOutputCategory NodeOutputCategory { get; set; }
        public string Label { get; set; }

        public NodeOutputControl()
        {
            InitializeComponent();
        }

        public NodeOutputControl(NodeOutput nodeOutput)
        {
            InitializeComponent();

            Label = nodeOutput.label;
            textLabel.Text = Label;

            NodeOutputCategory = nodeOutput.category;

            switch (NodeOutputCategory)
            {
                case NodeOutput.NodeOutputCategory.Undefined:
                    ellipseOut.Fill = new LinearGradientBrush(Color.FromRgb(185, 185, 185), Color.FromRgb(64, 64, 64), 90);
                    break;
                case NodeOutput.NodeOutputCategory.Image:
                    ellipseOut.Fill = new LinearGradientBrush(Color.FromRgb(255, 235, 40), Color.FromRgb(100, 90, 0), 90);
                    break;
                case NodeOutput.NodeOutputCategory.Number:
                    ellipseOut.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(90, 90, 90), 90);
                    break;
                default:
                    break;
            }
        }
    }
}
