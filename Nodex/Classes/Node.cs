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
        public NodeIO[] outputs { get; private set; }
        public NodeProperty[] properties { get; }
        public delegate object[] CalculateOutputs(NodeIO[] inputs, NodeProperty[] properties);
        public CalculateOutputs calculateOutputs { get; }

        public enum NodeCategory
        {
            Miscellaneous = 0,
            Input = 1,
            Texture = 2
        }

        public Node(NodeCategory category, string label, NodeIO[] inputs, NodeIO[] outputs, NodeProperty[] properties, CalculateOutputs calculateOutputs)
        {
            this.category = category;
            this.label = label;
            this.inputs = inputs;
            this.outputs = outputs;
            this.properties = properties;
            this.calculateOutputs = calculateOutputs;
        }

        public void PopulateOutputs()
        {
            object[] objects = calculateOutputs(inputs, properties);
            for (int i = 0; i < objects.Length; i++)
            {
                if (i > outputs.Length - 1)
                    throw new ArgumentException("Too many objects were returned.");
                outputs[i].value = objects[i];
            }
        }
    }
}
