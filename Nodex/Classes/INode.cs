using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes
{
    public interface INode
    {
        NodeControl nodeControl { get; set; }

        object[] Calculate(NodeIO[] inputs, NodeProperty[] properties);
    }
}
