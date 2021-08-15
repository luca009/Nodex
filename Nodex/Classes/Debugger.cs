using Nodex.Resources.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes
{
    public static class Debugger
    {
        public static void AddValue(object obj)
        {
            DebugWindow debugWindow = ((MainWindow)App.Current.MainWindow).debugWindow;
            debugWindow.listboxInfo.Items.Add(obj);
        }

        public static void Clear()
        {
            DebugWindow debugWindow = ((MainWindow)App.Current.MainWindow).debugWindow;
            debugWindow.listboxInfo.Items.Clear();
        }
    }
}
