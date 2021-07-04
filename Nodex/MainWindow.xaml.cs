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

namespace Nodex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NodeControl nodeControl = new NodeControl(new Node(Node.NodeCategory.Input,
                "test",
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Input), new NodeIO(NodeIO.NodeIOCategory.Number, "Number", NodeIO.NodeIOType.Input) },
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Undefined, "Undefined", NodeIO.NodeIOType.Output) },
                new NodeProperty[] { new NodeProperty(new Button(), "property") }))
                { Width = 200, Height = 200, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            gridNodeSpace.Children.Add(nodeControl);
        }
    }
}
