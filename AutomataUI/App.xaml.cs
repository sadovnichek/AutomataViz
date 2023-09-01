using System.IO;
using System.Windows;
using System.Windows.Threading;
using Application;
using Autofac;
using AutomataUI.Workspaces;
using Domain.Services;
using DotFormat;

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
            builder.RegisterType<VisualizationService>().As<IVisualizationService>().SingleInstance();
            builder.RegisterType<DotFormatBuilder>().As<IGraph>().SingleInstance();
            builder.RegisterType<NodeShape>().As<INodeShape>().SingleInstance();
            builder.RegisterType<RandomAutomataService>().As<IRandomAutomataService>().SingleInstance();
            builder.RegisterType<MinimizationAlgorithm>().As<IAlgorithm>().SingleInstance();
            builder.RegisterType<DeterminizationAlgorithm>().As<IAlgorithm>().SingleInstance();
            builder.RegisterType<WordRecognitionAlgorithm>().As<IAlgorithm>().SingleInstance();
            builder.RegisterType<LambdaClosureAlgorithm>().As<IAlgorithm>().SingleInstance();
            builder.RegisterType<AutomataParser>().As<IAutomataParser>().SingleInstance();
            builder.RegisterType<AutomataWorkspace>().As<IAutomataWorkspace>().SingleInstance();
            builder.RegisterType<WordInputWorkspace>().As<IWordInputWorkspace>().SingleInstance();
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