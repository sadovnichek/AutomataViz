using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace AutomataUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Произошла непредвиденная ошибка. Подробности в файле log.txt");
            File.WriteAllText("./log.txt", $"{e.Exception.Message}\n{e.Exception.StackTrace}");
            e.Handled = true;
        }
    }
}