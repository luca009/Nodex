using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nodex.Classes.Nodes
{
    public class TestNode : INode
    {
        public NodeControl nodeControl { get; set; }

        public TestNode()
        {
            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Miscellaneous,
                "Test",
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image1", NodeIO.NodeIOType.Input), new NodeIO(NodeIO.NodeIOCategory.Image, "Image2", NodeIO.NodeIOType.Input, true), new NodeIO(NodeIO.NodeIOCategory.Image, "Image3", NodeIO.NodeIOType.Input, true) },
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Output) },
                new NodeProperty[] { new NodeProperty(new IntegerUpDown(1, 3, 1, 1), "Image") },
                node.Calculate
                ), this.GetType())
            { Width = 160, Height = 110 };
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            NodeIO temp = inputs[((IntegerUpDown)properties[0].propertyElement).Value - 1];
            if (temp.value == null)
                return new object[] { 0 };

            return new object[] { temp.value };
        }
    }
}
