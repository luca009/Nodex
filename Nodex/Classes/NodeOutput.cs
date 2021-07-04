using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes
{
    public class NodeOutput
    {
        public NodeOutputCategory category { get; }
        public string label { get; }
        public bool optional { get; }

        public enum NodeOutputCategory
        {
            Undefined = 0,
            Image = 1,
            Number = 2
        }

        public NodeOutput(NodeOutputCategory category, string label)
        {
            this.category = category;
            this.label = label;
            this.optional = optional;
        }
    }
}
