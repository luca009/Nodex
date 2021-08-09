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
        public bool isConnected { get; private set; }
        public List<NodeInputControl> connectedNodeInputs { get; set; }
        public List<Line> connectedLines { get; set; }
        public Canvas parentCanvas { get; set; }
        public NodeIO nodeIO { get; private set; }
        public NodeControl parentNodeControl { get; private set; }
        public delegate void MouseLeftButtonDownHandler(object sender, MouseEventArgs e);
        public delegate void MouseLeftButtonUpHandler(object sender, MouseEventArgs e);
        public delegate void DropHandler(object sender, DragEventArgs e);
        public new event MouseLeftButtonDownHandler MouseLeftButtonDown;
        public new event MouseLeftButtonUpHandler MouseLeftButtonUp;
        public new event DropHandler Drop;

        /// <summary>
        /// This constructor is purely for the designer and should not be used as it renders the control useless.
        /// </summary>
        public NodeOutputControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a new NodeOutputControl (visual element that can output a value and be connected to multiple NodeInputControls).
        /// </summary>
        /// <param name="nodeIO">The nodeIO to base the control off of.</param>
        public NodeOutputControl(NodeIO nodeIO)
        {
            InitializeComponent();

            Label = nodeIO.label;
            textLabel.Text = Label;

            NodeIOCategory = nodeIO.category;
            this.nodeIO = nodeIO;

            connectedNodeInputs = new List<NodeInputControl>();
            connectedLines = new List<Line>();

            parentNodeControl = null;

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
                case NodeIO.NodeIOCategory.Vector:
                    ellipseOut.Fill = new LinearGradientBrush(Color.FromRgb(110, 50, 255), Color.FromRgb(40, 0, 130), 90);
                    break;
                default:
                    MessageBox.Show("Non-serious error, no Ellipse.Fill was defined for the NodeIOCategory in NodeOutputControl.");
                    break;
            }
        }

        public void RefreshNodeControl()
        {
            FrameworkElement currentElement = (FrameworkElement)this;
            do
            {
                currentElement = currentElement.Parent as FrameworkElement;
            } while (currentElement.GetType() != typeof(NodeControl));
            parentNodeControl = (NodeControl)currentElement;
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

        private void ellipseOut_Drop(object sender, DragEventArgs e)
        {
            if (Drop != null)
                Drop(sender, e);
            if (e.Source != null)
                isConnected = true;
        }
    }
}
