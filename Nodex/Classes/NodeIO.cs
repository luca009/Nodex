using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes
{
    public class NodeIO
    {
        public NodeIOCategory category { get; }
        public NodeIOType type { get; }
        public string label { get; }
        public bool optional { get; }
        public List<NodeIO> connectedNodeIOs { get; set; }
        public object value { get; set; }

        public enum NodeIOCategory
        { 
            Undefined = 0,
            Image = 1,
            Number = 2,
            Vector = 3
        }

        public enum NodeIOType
        {
            Input = 0,
            Output = 1
        }

        public NodeIO(NodeIOCategory category, string label, NodeIOType type, bool optional = false)
        {
            this.category = category;
            this.label = label;
            this.optional = optional;
            this.type = type;
        }
    }
}
