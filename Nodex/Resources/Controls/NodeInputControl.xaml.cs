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
    /// Interaction logic for NodeInput.xaml
    /// </summary>
    public partial class NodeInputControl : UserControl
    {
        public NodeInput.NodeInputCategory NodeInputCategory { get; set; }
        public string Label { get; set; }
        public new delegate void MouseLeftButtonDown(object sender, MouseEventArgs e);
        public new delegate void MouseLeftButtonUp(object sender, MouseEventArgs e);

        public NodeInputControl()
        {
            InitializeComponent();
        }

        public NodeInputControl(NodeInput nodeInput)
        {
            InitializeComponent();

            Label = nodeInput.label;
            textLabel.Text = Label;

            NodeInputCategory = nodeInput.category;

            switch (NodeInputCategory)
            {
                case NodeInput.NodeInputCategory.Undefined:
                    ellipseIn.Fill = new LinearGradientBrush(Color.FromRgb(185, 185, 185), Color.FromRgb(64, 64, 64), 90);
                    break;
                case NodeInput.NodeInputCategory.Image:
                    ellipseIn.Fill = new LinearGradientBrush(Color.FromRgb(255, 235, 40), Color.FromRgb(100, 90, 0), 90);
                    break;
                case NodeInput.NodeInputCategory.Number:
                    ellipseIn.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(90, 90, 90), 90);
                    break;
                default:
                    break;
            }
        }
    }
}
