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
        public event NodeMouseLeftButtonDownHandler NodeMouseLeftButtonDown;
        public event NodeMouseLeftButtonUpHandler NodeMouseLeftButtonUp;
        bool mouseDown = false;
        Point dragOffset;

        public NodeControl()
        {
            InitializeComponent();
        }
        public NodeControl(Node node)
        {
            InitializeComponent();

            Label = node.label;
            textLabel.Text = Label;

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

            foreach (NodeIO nodeInput in node.inputs)
            {
                stackpanelInputs.Children.Add(new NodeInputControl(nodeInput));
            }
            foreach (NodeIO nodeOutput in node.outputs)
            {
                stackpanelOutputs.Children.Add(new NodeOutputControl(nodeOutput));
            }
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
                MoveUserControl(e);
        }

        private void gridContent_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
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
