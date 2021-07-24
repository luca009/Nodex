using Nodex.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Nodex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            ((MainWindow)App.Current.MainWindow).gridExceptions.Visibility = Visibility.Visible;
            ((MainWindow)App.Current.MainWindow).textException.Text = e.Exception.Message;
            Thread thread = new Thread(() => {
                Thread.Sleep(7500);
                Dispatcher.Invoke(() => { ((MainWindow)App.Current.MainWindow).gridExceptions.Visibility = Visibility.Hidden; });
            });
            thread.Start();
            thread.IsBackground = true;

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
