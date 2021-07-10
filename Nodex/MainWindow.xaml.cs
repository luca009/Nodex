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

namespace Nodex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NodeControl selectedNode;
        Node outputNode;

        public MainWindow()
        {
            InitializeComponent();

            App.Current.Properties.Add("imageWidth", 512);
            App.Current.Properties.Add("imageHeight", 512);
            
            NodeControl nodeControl = new OutputNode().nodeControl;
            CreateNodeAndAdd(nodeControl);
            outputNode = nodeControl.node;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Random tempRandom = new Random();
            //NodeControl nodeControl = new NodeControl(new Node(Node.NodeCategory.Input,
            //    "test",
            //    new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Input), new NodeIO(NodeIO.NodeIOCategory.Number, "Number", NodeIO.NodeIOType.Input) },
            //    new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Undefined, "Undefined", NodeIO.NodeIOType.Output) },
            //    new NodeProperty[] { new NodeProperty(new Button(), tempRandom.Next().ToString()) },
            //    (NodeIO[] inputs, NodeProperty[] properties) => { return null; }))
            //    { Width = 200, Height = 200, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

            CreateNodeAndAdd(new SolidColorNode().nodeControl);
        }

        private void CreateNodeAndAdd(NodeControl nodeControl)
        {
            //Add event handlers
            nodeControl.NodeMouseLeftButtonDown += nodeControl_NodeMouseLeftButtonDown;
            nodeControl.NodeMouseLeftButtonUp += nodeControl_NodeMouseLeftButtonUp;
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
            nodeControl.NodeDrop += nodeControl_NodeDrop;

            //Add to the canvas and push it to the top
            canvasNodeSpace.Children.Add(nodeControl);
            selectedNode = nodeControl;
            Panel.SetZIndex(nodeControl, 3);
        }

        private void nodeControl_NodeMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Type lineType = typeof(Line);
            Type nodeControlType = typeof(NodeControl);
            foreach (FrameworkElement item in canvasNodeSpace.Children)
            {
                switch (item)
                {
                    case Line line:
                        Panel.SetZIndex(item, 1);
                        break;
                    case NodeControl nodeControl:
                        Panel.SetZIndex(item, 2);
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

            for (int i = stackpanelConfigureElements.Children.Count - 1; i >= 0; i--)
            {
                //stackpanelConfigureElements.RemoveChild(stackpanelConfigureElements.Children[i]);
                var temp = stackpanelConfigureElements.Children[i];
                ((Grid)temp).Children.Clear();
                this.RemoveLogicalChild(temp);
                //this.RemoveVisualChild(temp);
            }

            stackpanelConfigureElements.Children.Clear();
            foreach (NodeProperty property in nodeControl.node.properties)
            {
                Grid addGrid = new Grid();
                TextBlock labelText = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 5, 0), Text = property.label };
                addGrid.Children.Add(labelText);

                //string elementXaml = XamlWriter.Save(property.propertyElement);
                //StringReader stringReader = new StringReader(elementXaml);
                //XmlReader xmlReader = XmlReader.Create(stringReader);
                //Control newPropertyElement = (UserControl)XamlReader.Load(xmlReader);

                //newPropertyElement.HorizontalAlignment = HorizontalAlignment.Right;

                Control tempControl = (Control)property.propertyElement;
                this.RemoveLogicalChild(tempControl);
                tempControl.HorizontalAlignment = HorizontalAlignment.Right;
                addGrid.Children.Add(tempControl);
                stackpanelConfigureElements.Children.Add(addGrid);
            }
        }

        private void nodeIOControl_Drop(object sender, DragEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (null == element)
                return;
            IDataObject data = e.Data;

            //canvasNodeSpace.Children.RemoveAt(linePreviewIndex);

            NodeInputControl nodeInputControl;
            NodeOutputControl nodeOutputControl;

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
                nodeInputControl.connectedNodeOutput.connectedNodeInputs.Remove(nodeInputControl);
                canvasNodeSpace.Children.Remove(nodeInputControl.connectedLine);
                nodeInputControl.connectedNodeOutput = null;
                nodeInputControl.connectedLine = null;
            }

            nodeInputControl.connectedNodeOutput = nodeOutputControl;
            nodeInputControl.connectedLine = line;
            nodeOutputControl.connectedNodeInputs.Add(nodeInputControl);
            nodeOutputControl.connectedLines.Add(line);

            nodeInputControl.connectedLineIndexInCanvas = canvasNodeSpace.Children.Add(line);

            NetworkSolver.Solve(GetNodes(), imagePreview);

            e.Handled = true;
        }

        private Node[] GetNodes()
        {
            List<Node> nodes = new List<Node>();
            foreach (Control control in canvasNodeSpace.Children)
            {
                if (control.GetType() != typeof(NodeControl))
                    continue;
                nodes.Add(((NodeControl)control).node);
            }
            if (nodes.Count <= 0)
                return null;
            return nodes.ToArray();
        }

        public Node GetOutputNode()
        {
            foreach (Control control in canvasNodeSpace.Children)
            {
                if (control.GetType() != typeof(NodeControl))
                    continue;
                if (((NodeControl)control).nodeType == typeof(OutputNode))
                    return ((NodeControl)control).node;
            }
            return null;
        }

        private void nodeIOControl_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;

            if (((Grid)element.Parent).Parent is NodeInputControl)
            {
                /* Attempt at making a "preview" line while dragging
                Point mousePosition = e.GetPosition(canvasNodeSpace);
                Line linePreview = new Line()
                {
                    X1 = mousePosition.X,
                    Y1 = mousePosition.Y,
                    X2 = mousePosition.X,
                    Y2 = mousePosition.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                    StrokeThickness = 1
                };

                linePreviewIndex = canvasNodeSpace.Children.Add(linePreview);

                Thread thread = new Thread(() => {
                    while (true)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Point mousePositionCurrent = e.GetPosition(canvasNodeSpace);
                            linePreview.RecalculateConnection(new Point(linePreview.X1, linePreview.Y1), mousePositionCurrent);
                        });
                        Thread.Sleep(16); //Wait roughly as needed to achieve 60fps
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();*/
                //if (((NodeInputControl)((Grid)element.Parent).Parent).connectedNodeOutput != null)
                //{
                //    ((NodeInputControl)((Grid)element.Parent).Parent).connectedLine = null;
                //    canvasNodeSpace.Children.RemoveAt(((NodeInputControl)((Grid)element.Parent).Parent).connectedLineIndexInCanvas);
                //    ((NodeInputControl)((Grid)element.Parent).Parent).connectedLineIndexInCanvas = 0;
                //    ((NodeInputControl)((Grid)element.Parent).Parent).connectedNodeOutput = null;
                //}

                DragDrop.DoDragDrop(sender as UIElement, ((Grid)element.Parent).Parent as NodeInputControl, DragDropEffects.Link);
            }
            else
                DragDrop.DoDragDrop(sender as UIElement, ((Grid)element.Parent).Parent as NodeOutputControl, DragDropEffects.Link);
        }

        private void nodeControl_NodeDrop(object sender, DragEventArgs e)
        {
            //FrameworkElement elem = sender as FrameworkElement;
            //if (null == elem)
            //    return;
            //IDataObject data = e.Data;
            //if (data.GetDataPresent(typeof(NodeInputControl)) && !(sender is NodeInputControl))
            //{
            //    NodeInputControl nodeInputControl = data.GetData(typeof(NodeInputControl)) as NodeInputControl;
            //    NodeOutputControl nodeOutputControl = sender as NodeOutputControl;
            //    Line line = new Line()
            //    {
            //        X1 = nodeInputControl.Margin.Left,
            //        Y1 = nodeInputControl.Margin.Top,
            //        X2 = nodeOutputControl.Margin.Left,
            //        Y2 = nodeOutputControl.Margin.Top,
            //        Stroke = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
            //        StrokeThickness = 4
            //    };
            //    nodeInputControl.connectedNodeOutput = nodeOutputControl;
            //    nodeOutputControl.connectedNodeInputs.Add(nodeInputControl);
            //}
            //else if (data.GetDataPresent(typeof(NodeOutputControl)) && !(sender is NodeOutputControl))
            //{
            //    NodeOutputControl nodeOutputControl = data.GetData(typeof(NodeOutputControl)) as NodeOutputControl;
            //}
            //else return;
        }

        private void nodeControl_NodeMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            /* UIElement element = sender as UIElement;
             if (element == null)
                 return;
             if (element.GetType() == typeof(NodeInputControl))
                 DragDrop.DoDragDrop(element, new DataObject(element as NodeInputControl), DragDropEffects.Move);

             //else
             //    DragDrop.DoDragDrop(element, ((StackPanel)((Border)((Grid)((Border)((StackPanel)((NodeInputControl)((Grid)element).Parent).Parent).Parent).Parent).Children[2]).Child). as NodeOutputControl, DragDropEffects.Move);
             NodeControl nodeControl = element as NodeControl;*/
        }

        private void nodeControl_NodeMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void canvasNodeSpace_LayoutUpdated(object sender, EventArgs e)
        {
            /*foreach (object obj in canvasNodeSpace.Children)
            {
                if (obj.GetType() == typeof(NodeControl))
                    foreach (NodeInputControl nodeInputControl in ((NodeControl)obj).stackpanelInputs.Children)
                        nodeInputControl.connectedLine.RecalculateConnection(nodeInputControl.connectedNodeOutput, nodeInputControl);
            }*/
        }
    }
}
