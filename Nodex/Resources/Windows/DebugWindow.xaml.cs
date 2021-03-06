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
    /// Interaktionslogik für DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public ListBox infoListBox { get; set; }
        public DebugWindow()
        {
            InitializeComponent();
            infoListBox = listboxInfo;
        }
    }
}
