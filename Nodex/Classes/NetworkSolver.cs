using Nodex.Classes.Nodes;
using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Nodex.Classes
{
    public static class NetworkSolver
    {
        /// <summary>
        /// Solves the node network and sets the output node
        /// </summary>
        /// <param name="nodes">Node network as array</param>
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

        /// <summary>
        /// Solves the node network without reindexing it, saving some performance, though should not be used if the node network has changed since the last solve.
        /// </summary>
        /// <param name="nodes">Node network as array</param>
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

        /// <summary>
        /// Removes index of each node on a canvas
        /// </summary>
        private static void RemoveIndexes(Canvas parentCanvas)
        {
            foreach (FrameworkElement item in parentCanvas.Children) {
                if (item.GetType() == typeof(NodeControl))
                {
                    ((NodeControl)item).index = 0;
                }
            }
        }

        /// <summary>
        /// Indexes all nodes, think of this as the distance from the output node
        /// </summary>
        /// <returns>Dictionary containing the NodeControls and their index</returns>
        private static Dictionary<NodeControl, int> Index(NodeControl outputNode)
        {
            Dictionary<NodeControl, int> nodeControlIndexes = new Dictionary<NodeControl, int>();

            outputNode.index = 1;
            nodeControlIndexes.Add(outputNode, 1);

            NodeControl currentNodeControl = outputNode.inputs[0].connectedNodeOutput.parentNodeControl;
            int currentIndex = 2;

            var temp = IndexFromNodeControl(currentNodeControl, currentIndex, nodeControlIndexes);
            if (temp != null)
                nodeControlIndexes = temp;
            else
                return null;

            return nodeControlIndexes;
        }

        private static Dictionary<NodeControl, int> IndexFromNodeControl(NodeControl startingNodeControl, int startingIndex, Dictionary<NodeControl, int> nodeControlIndexes = null, short continueFromIndex = -1)
        {
            if (continueFromIndex != -1)
                startingNodeControl = startingNodeControl.inputs[continueFromIndex].connectedNodeOutput.parentNodeControl;

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
            }

            bool continueLoop = true;
            while (continueLoop)
            {
                switch (startingNodeControl.inputs.Count)
                {
                    case 0:
                        continueLoop = false;
                        if (startingNodeControl.index > 0)
                            nodeControlIndexes.Remove(startingNodeControl);
                        if (startingIndex > startingNodeControl.index)
                            startingNodeControl.index = startingIndex;
                        nodeControlIndexes.Add(startingNodeControl, startingIndex);
                        break;
                    case 1:
                        //There's only one input, continue from there
                        if (startingNodeControl.index > 0)
                            nodeControlIndexes.Remove(startingNodeControl);
                        if (startingIndex > startingNodeControl.index)
                            startingNodeControl.index = startingIndex;

                        nodeControlIndexes.Add(startingNodeControl, startingIndex);

                        if (startingNodeControl.inputs[0].connectedNodeOutput == null)
                            return nodeControlIndexes;
                        startingNodeControl = startingNodeControl.inputs[0].connectedNodeOutput.parentNodeControl;
                        break;
                    case int n when n > 1:
                        //There are multiple inputs, recursively continue from each one
                        bool alreadyAdded = false;
                        if (startingNodeControl.index > 0)
                        {
                            nodeControlIndexes.Remove(startingNodeControl);
                            alreadyAdded = true;
                        }
                        //Super janky way to detect nodes connected to themselves
                        if (startingIndex >= 2800)
                        {
                            startingNodeControl.invalidConnections = true;
                            return null;
                        }
                        if (startingIndex > startingNodeControl.index)
                            startingNodeControl.index = startingIndex;

                        nodeControlIndexes.Add(startingNodeControl, startingIndex);

                        int nulls = 0;
                        for (short i = 0; i < startingNodeControl.inputs.Count; i++)
                        {
                            if (startingNodeControl.inputs[i].connectedNodeOutput == null)
                            {
                                nulls++;
                                if (nulls >= startingNodeControl.inputs.Count)
                                    return nodeControlIndexes;
                                continue;
                            }

                            var temp = IndexFromNodeControl(startingNodeControl, startingIndex + 1, nodeControlIndexes, i);
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

        /// <summary>
        /// Loops through all nodes, populating all relevant outputs
        /// </summary>
        /// <param name="inputDictionary">Dictionary containing all NodeControls and their indexes</param>
        private static void CalculateOutput(Dictionary<NodeControl, int> inputDictionary)
        {
            if ((bool)App.Current.Properties["imageCalculatingThreadRunning"])
            {
                ((Thread)App.Current.Properties["imageCalculatingThread"]).Abort();
                App.Current.Properties["imageCalculatingThreadRunning"] = false;
            }

            var tempList = inputDictionary.ToList();
            //Thread thread = new Thread(() => {
                tempList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                tempList.Reverse();

                foreach (KeyValuePair<NodeControl, int> keyValuePair in tempList)
                {
                    Node node = null;
                    App.Current.Dispatcher.Invoke(() => { node = keyValuePair.Key.node; });
                    node.PopulateOutputs();
                }
            //});
            //App.Current.Properties["imageCalculatingThread"] = thread;
            App.Current.Properties["imageCalculatingThreadRunning"] = true;
            //thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            //thread.Start();
            //await Task.Run(() =>
            //{
                //thread.Join();
                //Thread.Sleep(2);
                App.Current.Properties["imageCalculatingThreadRunning"] = false;
            //});
        }
    }
}
