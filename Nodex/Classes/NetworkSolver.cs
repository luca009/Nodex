using Nodex.Classes.Nodes;
using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Nodex.Classes
{
    public static class NetworkSolver
    {
        public static void Solve(Node[] nodes)
        {
            NodeControl outputNode = ((OutputNode)App.Current.Properties["outputNode"]).nodeControl;
            if (outputNode.inputs[0].connectedNodeOutput == null)
                return;

            RemoveIndexes(((MainWindow)App.Current.MainWindow).canvasNodeSpace);

            Dictionary<NodeControl, int> indexDictionary = Index(outputNode);

            if (indexDictionary == null)
            {
                MessageBox.Show("Invalid Node Connections!", "Invalid!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CalculateOutput(indexDictionary);
        }

        public static void SolveWithoutReindexing(Node[] nodes)
        {
            Dictionary<NodeControl, int> indexDictionary = new Dictionary<NodeControl, int>();

            foreach (FrameworkElement item in ((MainWindow)App.Current.MainWindow).canvasNodeSpace.Children)
                if (item.GetType() == typeof(NodeControl))
                    indexDictionary.Add((NodeControl)item, ((NodeControl)item).index);

            if (indexDictionary == null)
            {
                MessageBox.Show("Invalid Node Connections!", "Invalid!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CalculateOutput(indexDictionary);
        }

        private static void RemoveIndexes(Canvas parentCanvas)
        {
            foreach (FrameworkElement item in parentCanvas.Children) {
                if (item.GetType() == typeof(NodeControl))
                {
                    ((NodeControl)item).index = 0;
                }
            }
        }

        private static Dictionary<NodeControl, int> Index(NodeControl outputNode)
        {
            Dictionary<NodeControl, int> nodeControlIndexes = new Dictionary<NodeControl, int>();

            outputNode.index = 1;
            nodeControlIndexes.Add(outputNode, 1);
            //outputNode.textLabel.Text = outputNode.index.ToString();
            //handledNodeControls++;

            NodeControl currentNodeControl = outputNode.inputs[0].connectedNodeOutput.parentNodeControl;
            int currentIndex = 2;

            var temp = IndexFromNodeControl(currentNodeControl, currentIndex, nodeControlIndexes);
            if (temp != null)
                nodeControlIndexes = temp;
            else
                return null;

            return nodeControlIndexes;
        }

        private static Dictionary<NodeControl, int> IndexFromNodeControl(NodeControl startingNodeControl, int startingIndex, Dictionary<NodeControl, int> nodeControlIndexes = null)
        {
            if (nodeControlIndexes == null)
                nodeControlIndexes = new Dictionary<NodeControl, int>();

            if (startingIndex >= 2800)
            {
                startingNodeControl.invalidConnections = true;
                return null;
            }

            if (startingNodeControl.inputs.Count <= 0)
            {
                if (startingNodeControl.index > 0)
                    nodeControlIndexes.Remove(startingNodeControl);

                nodeControlIndexes.Add(startingNodeControl, startingIndex);

                if (startingIndex > startingNodeControl.index)
                    startingNodeControl.index = startingIndex;
                //startingNodeControl.textLabel.Text = startingNodeControl.index.ToString();
            }

            while (startingNodeControl.inputs.Count > 0)
            {
                switch (startingNodeControl.inputs.Count)
                {
                    case 0: break;
                    case 1:
                        if (startingNodeControl.index > 0)
                            nodeControlIndexes.Remove(startingNodeControl);
                        if (startingIndex > startingNodeControl.index)
                            startingNodeControl.index = startingIndex;

                        nodeControlIndexes.Add(startingNodeControl, startingIndex);
                        //startingNodeControl.textLabel.Text = startingNodeControl.index.ToString();

                        startingNodeControl = startingNodeControl.inputs[0].parentNodeControl;
                        break;
                    case int n when n > 1:
                        bool alreadyAdded = false;
                        if (startingNodeControl.index > 0)
                        {
                            nodeControlIndexes.Remove(startingNodeControl);
                            alreadyAdded = true;
                        }
                        if (startingIndex >= 2800)
                        {
                            startingNodeControl.invalidConnections = true;
                            return null;
                        }
                        if (startingIndex > startingNodeControl.index)
                            startingNodeControl.index = startingIndex;

                        nodeControlIndexes.Add(startingNodeControl, startingIndex);
                        //startingNodeControl.textLabel.Text = startingNodeControl.index.ToString();

                        int nulls = 0;
                        for (int i = 0; i < startingNodeControl.inputs.Count; i++)
                        {
                            if (startingNodeControl.inputs[i].connectedNodeOutput == null)
                            {
                                nulls++;
                                if (nulls >= startingNodeControl.inputs.Count)
                                    return nodeControlIndexes;
                                continue;
                            }

                            var temp = IndexFromNodeControl(startingNodeControl.inputs[i].connectedNodeOutput.parentNodeControl, startingIndex + 1, nodeControlIndexes);
                            if (temp == null)
                                return null;
                            nodeControlIndexes = temp;
                        }

                        if (alreadyAdded)
                            return nodeControlIndexes;

                        break;
                    default:
                        throw new Exception("wtf");
                }

                startingIndex++;
            }

            return nodeControlIndexes;
        }

        private static void CalculateOutput(Dictionary<NodeControl, int> inputDictionary)
        {
            var tempList = inputDictionary.ToList();
            tempList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            tempList.Reverse();

            foreach (KeyValuePair<NodeControl, int> keyValuePair in tempList)
            {
                NodeControl nodeControl = keyValuePair.Key;
                nodeControl.node.PopulateOutputs();
            }
        }

        private static int GetAmountOfNodeControlsInCanvas(Canvas canvas)
        {
            int count = 0;
            foreach (FrameworkElement item in canvas.Children)
                if (item.GetType() == typeof(NodeControl))
                    count++;
            return count;
        }
    }
}
