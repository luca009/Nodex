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
using Nodex.Resources.Controls;
using Nodex.Resources;
using System.Threading;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using Nodex.Classes.Nodes;
using System.Windows.Media.Effects;

namespace Nodex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NodeControl selectedNode;
        public OutputNode outputNode { get; private set; }
        public Node[] lastNodes { get; private set; }

        #region Commands
        private ICommand deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand
                    ?? (deleteCommand = new ActionCommand(() =>
                    {
                        DeleteNode(selectedNode);
                    }));
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            OutputNode outputNode = new OutputNode();
            NodeControl nodeControl = outputNode.nodeControl;
            CreateNodeAndAdd(nodeControl);
            this.outputNode = outputNode;

            App.Current.Properties.Add("imageWidth", 512);
            App.Current.Properties.Add("imageHeight", 512);
            App.Current.Properties.Add("outputNode", outputNode);
        }

        private void bSolidColor_Click(object sender, RoutedEventArgs e)
        {
            CreateNodeAndAdd(new SolidColorNode().nodeControl);
        }

        private void bTest_Click(object sender, RoutedEventArgs e)
        {
            CreateNodeAndAdd(new TestNode().nodeControl);
        }

        private void bWhiteNoise_Click(object sender, RoutedEventArgs e)
        {
            CreateNodeAndAdd(new WhiteNoiseNode().nodeControl);
        }

        private void CreateNodeAndAdd(NodeControl nodeControl)
        {
            //Add event handlers
            nodeControl.NodeMouseRightButtonDown += nodeControl_NodeMouseRightButtonDown;
            foreach (NodeInputControl nodeInputControl in nodeControl.stackpanelInputs.Children)
            {
                nodeInputControl.MouseLeftButtonDown += nodeIOControl_MouseLeftButtonDown;
                nodeInputControl.Drop += nodeIOControl_Drop;
            }
            foreach (NodeOutputControl nodeOutputControl in nodeControl.stackpanelOutputs.Children)
            {
                nodeOutputControl.MouseLeftButtonDown += nodeIOControl_MouseLeftButtonDown;
                nodeOutputControl.Drop += nodeIOControl_Drop;
            }

            //Add to the canvas and push it to the top
            canvasNodeSpace.Children.Add(nodeControl);
            selectedNode = nodeControl;
            Panel.SetZIndex(nodeControl, 3);
        }

        private void DeleteNode(NodeControl nodeControl)
        {
            if (nodeControl == null || nodeControl.nodeType == typeof(OutputNode))
                return;

            foreach (NodeInputControl nodeInputControl in nodeControl.inputs)
            {
                nodeInputControl.connectedNodeOutput.connectedNodeInputs = null;
                nodeInputControl.connectedNodeOutput.nodeIO.connectedNodeIOs.Remove(nodeInputControl.nodeIO);
                canvasNodeSpace.Children.Remove(nodeInputControl.connectedLine);
                nodeInputControl.connectedLine = null;
            }
            foreach (NodeOutputControl nodeOutputControl in nodeControl.outputs)
            {
                foreach (NodeInputControl nodeInputControl in nodeOutputControl.connectedNodeInputs)
                {
                    nodeInputControl.connectedNodeOutput = null;
                    nodeInputControl.nodeIO.connectedNodeIOs.Remove(nodeOutputControl.nodeIO);
                    canvasNodeSpace.Children.Remove(nodeInputControl.connectedLine);
                    nodeInputControl.connectedLine = null;
                }
            }

            //Cleanly empty the StackPanel for node properties
            for (int i = stackpanelConfigureElements.Children.Count - 1; i >= 0; i--)
            {
                var temp = stackpanelConfigureElements.Children[i];
                ((Grid)temp).Children.Clear();
                this.RemoveLogicalChild(temp);
            }
            stackpanelConfigureElements.Children.Clear();

            canvasNodeSpace.Children.Remove(nodeControl);
        }

        private void nodeControl_NodeMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Change ZIndexes of each object depending on their type (make NodeControls on top)
            Type lineType = typeof(Line);
            Type nodeControlType = typeof(NodeControl);
            foreach (UIElement item in canvasNodeSpace.Children)
            {
                switch (item)
                {
                    case Line line:
                        Panel.SetZIndex(item, 1);
                        break;
                    case NodeControl nodeControl:
                        Panel.SetZIndex(item, 2);
                        DropShadowEffect dropShadowEffectOfNodeControl = (DropShadowEffect)((UIElement)nodeControl.Content).Effect;
                        dropShadowEffectOfNodeControl.Color = Color.FromRgb(0, 0, 0);
                        dropShadowEffectOfNodeControl.BlurRadius = 5;
                        dropShadowEffectOfNodeControl.Opacity = 100;
                        break;
                    default:
                        Panel.SetZIndex(item, 0);
                        break;
                }
            }


            //Cycle through the parents of the sender and find the related NodeControl
            FrameworkElement currentElement = (FrameworkElement)sender;
            do
            {
                currentElement = currentElement.Parent as FrameworkElement;
            } while (currentElement.GetType() != typeof(NodeControl));

            SetNodeSelection((NodeControl)currentElement);
        }

        private void SetNodeSelection(NodeControl nodeControl)
        {
            Panel.SetZIndex(nodeControl, 3);

            //Cleanly empty the StackPanel for node properties
            for (int i = stackpanelConfigureElements.Children.Count - 1; i >= 0; i--)
            {
                var temp = stackpanelConfigureElements.Children[i];
                ((Grid)temp).Children.Clear();
                this.RemoveLogicalChild(temp);
            }
            stackpanelConfigureElements.Children.Clear();

            //Add the new node properties to the StackPanel
            foreach (NodeProperty property in nodeControl.node.properties)
            {
                Grid addGrid = new Grid();
                TextBlock labelText = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 5, 0), Text = property.label };
                addGrid.Children.Add(labelText);

                Control tempControl = (Control)property.propertyElement;
                this.RemoveLogicalChild(tempControl);
                tempControl.HorizontalAlignment = HorizontalAlignment.Right;
                addGrid.Children.Add(tempControl);
                stackpanelConfigureElements.Children.Add(addGrid);
            }

            selectedNode = nodeControl;

            DropShadowEffect dropShadowEffectOfNodeControl = (DropShadowEffect)((UIElement)nodeControl.Content).Effect;
            dropShadowEffectOfNodeControl.Color = Color.FromRgb(0, 200, 240);
            dropShadowEffectOfNodeControl.BlurRadius = 2;
            dropShadowEffectOfNodeControl.Opacity = 40;
        }

        private void nodeIOControl_Drop(object sender, DragEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (null == element)
                return;
            IDataObject data = e.Data;

            NodeInputControl nodeInputControl;
            NodeOutputControl nodeOutputControl;

            //Get the affected NodeInput-/OutputControls
            if (data.GetDataPresent(typeof(NodeInputControl)) && ((Grid)element.Parent).Parent is NodeOutputControl)
            {
                nodeInputControl = data.GetData(typeof(NodeInputControl)) as NodeInputControl;
                nodeOutputControl = ((Grid)element.Parent).Parent as NodeOutputControl;
            }
            else if (data.GetDataPresent(typeof(NodeOutputControl)) && ((Grid)element.Parent).Parent is NodeInputControl)
            {
                nodeInputControl = ((Grid)element.Parent).Parent as NodeInputControl;
                nodeOutputControl = data.GetData(typeof(NodeOutputControl)) as NodeOutputControl;
            }
            else return;

            //Cycle through the parents of the NodeOutput (doesn't matter if Output or Input) and find the Canvas it's on
            FrameworkElement currentElement = nodeOutputControl;
            do
            {
                currentElement = currentElement.Parent as FrameworkElement;
            } while (currentElement.GetType() != typeof(Canvas));
            Canvas canvasParent = currentElement as Canvas;

            //Get the two connecting ellipses' centers
            Ellipse nodeInputControlEllipse = (Ellipse)((Grid)nodeInputControl.Content).Children[0];
            Ellipse nodeOutputControlEllipse = (Ellipse)((Grid)nodeOutputControl.Content).Children[1];
            Point nodeInputControlRelativePoint = nodeInputControlEllipse.TranslatePoint(new Point(nodeInputControlEllipse.Width / 2, nodeInputControlEllipse.Height / 2), canvasParent);
            Point nodeOutputControlRelativePoint = nodeOutputControlEllipse.TranslatePoint(new Point(nodeOutputControlEllipse.Width / 2, nodeOutputControlEllipse.Height / 2), canvasParent);

            Line line = new Line()
            {
                X1 = nodeInputControlRelativePoint.X,
                Y1 = nodeInputControlRelativePoint.Y,
                X2 = nodeOutputControlRelativePoint.X,
                Y2 = nodeOutputControlRelativePoint.Y,
                Stroke = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                StrokeThickness = 4
            };

            //Clear a potentially already existing line
            if (nodeInputControl.connectedLine != null)
            {
                nodeInputControl.connectedNodeOutput.connectedLines.Remove(nodeInputControl.connectedLine);
                if (nodeOutputControl.nodeIO.connectedNodeIOs != null)
                    nodeOutputControl.nodeIO.connectedNodeIOs.Remove(nodeInputControl.nodeIO);
                nodeInputControl.connectedNodeOutput.connectedNodeInputs.Remove(nodeInputControl);
                canvasNodeSpace.Children.Remove(nodeInputControl.connectedLine);
                nodeInputControl.connectedNodeOutput = null;
                nodeInputControl.connectedLine = null;
            }

            //Add the new line and node connections
            nodeInputControl.connectedNodeOutput = nodeOutputControl;
            nodeInputControl.nodeIO.connectedNodeIOs = new List<NodeIO> { nodeOutputControl.nodeIO };
            nodeInputControl.connectedLine = line;
            nodeOutputControl.connectedNodeInputs.Add(nodeInputControl);
            if (nodeOutputControl.nodeIO.connectedNodeIOs == null)
                nodeOutputControl.nodeIO.connectedNodeIOs = new List<NodeIO>();
            nodeOutputControl.nodeIO.connectedNodeIOs.Add(nodeInputControl.nodeIO);
            nodeOutputControl.connectedLines.Add(line);

            canvasNodeSpace.Children.Add(line);

            //Solve the network
            NetworkSolver.Solve(GetNodes());

            e.Handled = true;
        }

        /// <summary>
        /// Get all the Nodes on the Canvas
        /// </summary>
        private Node[] GetNodes()
        {
            List<Node> nodes = new List<Node>();
            foreach (object obj in canvasNodeSpace.Children)
            {
                if (obj.GetType() != typeof(NodeControl))
                    continue;
                nodes.Add(((NodeControl)obj).node);
            }
            if (nodes.Count <= 0)
                return null;

            Node[] nodesArray = nodes.ToArray();
            lastNodes = nodesArray;
            return nodesArray;
        }

        private void nodeIOControl_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;

            if (((Grid)element.Parent).Parent is NodeInputControl)
            {
                scrollviewerNodeSpace.AllowDrop = true;
                DragDrop.DoDragDrop(sender as UIElement, ((Grid)element.Parent).Parent as NodeInputControl, DragDropEffects.Link);
            }
            else
                DragDrop.DoDragDrop(sender as UIElement, ((Grid)element.Parent).Parent as NodeOutputControl, DragDropEffects.Link);
        }

        private void scrollviewerNodeSpace_Drop(object sender, DragEventArgs e)
        {
            //If a node connection is dragged directly onto the canvas, remove it
            FrameworkElement element = sender as FrameworkElement;
            if (null == element)
                return;
            IDataObject data = e.Data;

            if (data.GetDataPresent(typeof(NodeInputControl)))
            {
                NodeInputControl nodeInputControl = (NodeInputControl)data.GetData(typeof(NodeInputControl));
                nodeInputControl.connectedNodeOutput.connectedLines.Remove(nodeInputControl.connectedLine);
                nodeInputControl.nodeIO.connectedNodeIOs[0].connectedNodeIOs.Remove(nodeInputControl.nodeIO);
                nodeInputControl.nodeIO.connectedNodeIOs.Clear();
                nodeInputControl.connectedNodeOutput.connectedNodeInputs.Remove(nodeInputControl);
                canvasNodeSpace.Children.Remove(nodeInputControl.connectedLine);
                nodeInputControl.connectedNodeOutput = null;
                nodeInputControl.connectedLine = null;
            }
            scrollviewerNodeSpace.AllowDrop = false;

            NetworkSolver.Solve(GetNodes());
        }

        private void scrollviewerNodeSpace_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (selectedNode == null)
            //    return;

            ////Cleanly empty the StackPanel for node properties
            //for (int i = stackpanelConfigureElements.Children.Count - 1; i >= 0; i--)
            //{
            //    var temp = stackpanelConfigureElements.Children[i];
            //    ((Grid)temp).Children.Clear();
            //    this.RemoveLogicalChild(temp);
            //}
            //stackpanelConfigureElements.Children.Clear();

            //DropShadowEffect dropShadowEffectOfNodeControl = (DropShadowEffect)((UIElement)selectedNode.Content).Effect;
            //dropShadowEffectOfNodeControl.Color = Color.FromRgb(0, 200, 240);
            //dropShadowEffectOfNodeControl.BlurRadius = 2;
            //dropShadowEffectOfNodeControl.Opacity = 40;

            //selectedNode = null;
        }
    }
}
