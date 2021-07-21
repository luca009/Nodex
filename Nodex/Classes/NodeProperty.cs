using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Nodex.Classes
{
    public class NodeProperty
    {
        public UIElement propertyElement { get; }
        public string label { get; }
        public delegate void ValueUpdateHandler(object sender);
        public event ValueUpdateHandler ValueUpdate;

        public NodeProperty(UIElement propertyElement, string label)
        {
            this.propertyElement = propertyElement;
            this.label = label;

            propertyElement.MouseMove += ValueUpdated;
            propertyElement.MouseDown += ValueUpdated;
            propertyElement.MouseUp += ValueUpdated;
            propertyElement.MouseWheel += ValueUpdated;
            propertyElement.KeyDown += ValueUpdated;
            propertyElement.KeyUp += ValueUpdated;
        }

        private void ValueUpdated(object sender, MouseEventArgs e)
        {
            NetworkSolver.SolveWithoutReindexing(((MainWindow)App.Current.MainWindow).lastNodes);
        }
        private void ValueUpdated(object sender, KeyboardEventArgs e)
        {
            NetworkSolver.SolveWithoutReindexing(((MainWindow)App.Current.MainWindow).lastNodes);
        }
    }
}
