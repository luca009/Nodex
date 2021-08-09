using Nodex.Resources.Controls;
using System;
using System.Drawing;
using System.Numerics;

namespace Nodex.Classes.Nodes.Inputs
{
    public class VectorNode : INode
    {
        public NodeControl nodeControl { get; set; }

        public VectorNode()
        {
            INode node = this;
            nodeControl = new NodeControl(new Node(
                Node.NodeCategory.Texture,
                "Vector",
                new NodeIO[0] ,
                new NodeIO[] { new NodeIO(NodeIO.NodeIOCategory.Vector, "Vector", NodeIO.NodeIOType.Output) },
                new NodeProperty[] { new NodeProperty(new Vector4Control() { Height = 96 }, "Vector") },
                node.Calculate
                ), this.GetType())
            { Width = 80, Height = 70 };
        }

        object[] INode.Calculate(NodeIO[] inputs, NodeProperty[] properties)
        {
            Vector4 vector = ((Vector4Control)properties[0].propertyElement).Vector;
            Debugger.AddValue("vector node: " + vector);
            return new object[] { vector };
        }
    }
}
