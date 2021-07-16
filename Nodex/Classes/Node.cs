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
            foreach (NodeIO input in inputs)
            {
                //Check if all non-optional inputs are connected, if not, output 0
                if ((input.connectedNodeIOs == null || input.connectedNodeIOs.Count <= 0) && !input.optional)
                {
                    foreach (NodeIO output in outputs)
                    {
                        output.value = 0;
                    }
                    return;
                }
            }

            object[] objects = calculateOutputs(inputs, properties);

            for (int i = 0; i < objects.Length; i++)
            {
                if (i > outputs.Length - 1)
                    throw new ArgumentException("Too many objects were returned.");
                outputs[i].value = objects[i];

                foreach (NodeIO nodeIO in outputs[i].connectedNodeIOs)
                    nodeIO.value = objects[i];
            }
        }
    }
}
