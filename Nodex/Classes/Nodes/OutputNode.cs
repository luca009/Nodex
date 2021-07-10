using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes.Nodes
{
    class OutputNode : INode
    {
        public NodeControl nodeControl { get; set; }

        public OutputNode()
        {
            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                    "Output",
                    new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Image, "Image", NodeIO.NodeIOType.Input) },
                    new NodeIO[0],
                    new NodeProperty[] { new NodeProperty(new IntegerUpDown(512, 16384, 0, 64) { Height = 24 }, "Output Width"), new NodeProperty(new IntegerUpDown(512, 16384, 0, 64) { Height = 24 }, "Output Height") },
                    node.Calculate
                    ), this.GetType())
            { Width = 120, Height = 70 };
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            if (inputs[0].value == null)
                throw new Exception("No image was supplied to the output node despite being connected.");

            return new object[] { inputs[0].value };
        }
    }
}
