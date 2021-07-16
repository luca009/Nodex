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
    /// Interaction logic for NodeControl.xaml
    /// </summary>
    public partial class NodeControl : UserControl
    {
        public Node.NodeCategory NodeCategory { get; set; }
        public string Label { get; set; }
        public delegate void NodeMouseLeftButtonDownHandler(object sender, MouseEventArgs e);
        public delegate void NodeMouseLeftButtonUpHandler(object sender, MouseEventArgs e);
        public delegate void NodeMouseRightButtonDownHandler(object sender, MouseButtonEventArgs e);
        public delegate void NodeDropHandler(object sender, DragEventArgs e);
        public event NodeMouseLeftButtonDownHandler NodeMouseLeftButtonDown;
        public event NodeMouseLeftButtonUpHandler NodeMouseLeftButtonUp;
        public event NodeMouseRightButtonDownHandler NodeMouseRightButtonDown;
        public event NodeDropHandler NodeDrop;
        public Node node { get; }
        public Type nodeType { get; }
        public List<NodeInputControl> inputs { get; }
        public List<NodeOutputControl> outputs { get; }
        public int index { get; set; }
        public bool invalidConnections { get; set; }
        bool mouseDown = false;
        Point dragOffset;

        public NodeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a new NodeControl (control that houses multiple NodeInput-/NodeOutputControls and can be dragged around by the user).
        /// </summary>
        /// <param name="node">The node to base the control off of.</param>
        public NodeControl(Node node, Type nodeType)
        {
            InitializeComponent();

            Label = node.label;
            textLabel.Text = Label;
            this.node = node;

            NodeCategory = node.category;
            switch (NodeCategory)
            {
                case Node.NodeCategory.Miscellaneous:
                    canvasLabel.Background = new LinearGradientBrush(Color.FromRgb(50, 50, 50), Color.FromRgb(150, 150, 150), 90);
                    break;
                case Node.NodeCategory.Input:
                    canvasLabel.Background = new LinearGradientBrush(Color.FromRgb(0, 120, 20), Color.FromRgb(0, 255, 50), 90);
                    break;
                case Node.NodeCategory.Texture:
                    canvasLabel.Background = new LinearGradientBrush(Color.FromRgb(45, 60, 190), Color.FromRgb(0, 30, 160), 90);
                    break;
                default:
                    break;
            }

            inputs = new List<NodeInputControl>();
            outputs = new List<NodeOutputControl>();

            if (node.inputs != null)
                foreach (NodeIO nodeInput in node.inputs)
                {
                    NodeInputControl nodeInputControl = new NodeInputControl(nodeInput);
                    nodeInputControl.MouseLeftButtonDown += nodeIOControl_MouseLeftButtonDown;
                    nodeInputControl.MouseLeftButtonUp += nodeIOControl_MouseLeftButtonUp;
                    stackpanelInputs.Children.Add(nodeInputControl);
                    inputs.Add(nodeInputControl);
                    nodeInputControl.RefreshNodeControl();
                }
            if (node.outputs != null)
                foreach (NodeIO nodeOutput in node.outputs)
                {
                    NodeOutputControl nodeOutputControl = new NodeOutputControl(nodeOutput);
                    nodeOutputControl.MouseLeftButtonDown += nodeIOControl_MouseLeftButtonDown;
                    nodeOutputControl.MouseLeftButtonUp += nodeIOControl_MouseLeftButtonUp;
                    stackpanelOutputs.Children.Add(nodeOutputControl);
                    outputs.Add(nodeOutputControl);
                    nodeOutputControl.RefreshNodeControl();
                }

            if (nodeType != null)
                this.nodeType = nodeType;
        }

        private void nodeIOControl_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (NodeMouseLeftButtonDown != null)
                NodeMouseLeftButtonDown(sender, e);
        }

        private void nodeIOControl_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (NodeMouseLeftButtonUp != null)
                NodeMouseLeftButtonUp(sender, e);
        }

        private void nodeIOControl_Drop(object sender, DragEventArgs e)
        {
            if (NodeDrop != null)
                NodeDrop(sender, e);
        }

        private void MoveUserControl(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(gridContent);
            Double newX = LocalTranslateTransform.X + (mousePos.X - dragOffset.X);
            Double newY = LocalTranslateTransform.Y + (mousePos.Y - dragOffset.Y);
            LocalTranslateTransform.X = newX;
            LocalTranslateTransform.Y = newY;
        }

        private void gridContent_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                MoveUserControl(e);
                foreach (NodeOutputControl nodeOutputControl in stackpanelOutputs.Children)
                {
                    foreach (NodeInputControl nodeInputControl in nodeOutputControl.connectedNodeInputs)
                    {
                        nodeInputControl.connectedLine.RecalculateConnection(nodeOutputControl, nodeInputControl);
                    }
                }
                foreach (NodeInputControl nodeInputControl in stackpanelInputs.Children)
                {
                    nodeInputControl.connectedLine.RecalculateConnection(nodeInputControl.connectedNodeOutput, nodeInputControl);
                }
            }
        }

        private void gridContent_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (NodeMouseRightButtonDown != null)
                NodeMouseRightButtonDown(sender, e);
            mouseDown = true;
            dragOffset = e.GetPosition(sender as Grid);
            ((Grid)sender).CaptureMouse();
        }

        private void gridContent_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
            gridContent.ReleaseMouseCapture();
        }
    }
}
