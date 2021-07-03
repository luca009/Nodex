using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes
{
    class NodeInput
    {
        public NodeInputCategory category { get; }
        public string label { get; }
        public bool optional { get; }

        public enum NodeInputCategory
        {
            Undefined = 0,
            Image = 1,
            Number = 2
        }

        public NodeInput(NodeInputCategory category, string label, bool optional = true)
        {
            this.category = category;
            this.label = label;
            this.optional = optional;
        }
    }
}
