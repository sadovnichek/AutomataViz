using System.IO;
using System.Windows;
using System.Windows.Threading;
using Application;
using Autofac;
using AutomataUI.Workspaces;
using Domain.Services;

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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var container = ConfigureContainer();
            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        private IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ServiceResolver>().As<IServiceResolver>();
            builder.RegisterType<AutomataParser>().As<IAutomataParser>();
            builder.RegisterType<WorkspaceResolver>().As<IWorkspaceResolver>();
            builder.RegisterType<MainWindow>().AsSelf();
            return builder.Build();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Произошла неопознанная ошибка. Подробности в файле log.txt");
            File.WriteAllText("./log.txt", $"{e.Exception.Message}\n{e.Exception.StackTrace}");
            e.Handled = true;
        }
    }
}