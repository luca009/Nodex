using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nodex.Classes
{
    public class NodeProperty
    {
        public UIElement propertyElement { get; }
        public string label { get; }

        public NodeProperty(UIElement propertyElement, string label)
        {
            this.propertyElement = propertyElement;
            this.label = label;
        }
    }
}
