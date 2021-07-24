using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nodex.Resources.Windows
{
    /// <summary>
    /// Interaction logic for InitializingNode.xaml
    /// </summary>
    public partial class InitializingNode : Window
    {
        public int ChunkCount { get; private set; }
        int currentProgress = 0;

        public InitializingNode()
        {
            InitializeComponent();
        }

        public void SetChunkCount(int chunkCount)
        {
            ChunkCount = chunkCount;
            pbarProgress.Maximum = ChunkCount;
            textCalculatingChunk.Text = $"Calculating Chunk {currentProgress}/{ChunkCount}";
        }

        public void AddOneToProgress()
        {
            
            pbarProgress.Value++;
            currentProgress++;
            textCalculatingChunk.Text = $"Calculating Chunk {currentProgress}/{ChunkCount}";

            if (currentProgress >= ChunkCount)
                this.Close();
        }
    }
}
