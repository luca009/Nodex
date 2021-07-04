using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodex.Classes;

namespace Nodex.Classes
{
    public class Node
    {
        public NodeCategory category { get; }
        public string label { get; }
        public NodeIO[] inputs { get; }
        public NodeIO[] outputs { get; }
        public NodeProperty[] properties { get; }

        public enum NodeCategory
        {
            Miscellaneous = 0,
            Input = 1,
            Texture = 2
        }

        public Node(NodeCategory category, string label, NodeIO[] inputs, NodeIO[] outputs, NodeProperty[] properties)
        {
            this.category = category;
            this.label = label;
            this.inputs = inputs;
            this.outputs = outputs;
            this.properties = properties;
        }
    }
}
