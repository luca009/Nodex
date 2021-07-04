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
                new NodeInput[] { new NodeInput(NodeInput.NodeInputCategory.Image, "Image"), new NodeInput(NodeInput.NodeInputCategory.Number, "Number") },
                new NodeOutput[] { new NodeOutput(NodeOutput.NodeOutputCategory.Undefined, "Undefined") },
                new NodeProperty[] { new NodeProperty(new Button(), "property") }))
                { Width = 200, Height = 200, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            gridNodeSpace.Children.Add(nodeControl);
        }
    }
}
