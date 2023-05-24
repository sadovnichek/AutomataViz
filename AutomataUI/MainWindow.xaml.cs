using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Domain.Algorithm;
using Domain.Automatas;
using AutomataUI.Workspaces;
using Application;

namespace AutomataUI;

public partial class MainWindow
{
    //TODO:
    // Объявление воркспейсов в начале
    // Названия методом в соответствии со стилем
    // Метод ApplyButton слишком большой
    // Подумать над расширяемостью OnDropDownClosed
    // SRP в методе GenerateImage
    
    private readonly ScaleTransform st = new();
    private readonly InputWordWorkspace inputWordWorkspace = InputWordWorkspace.GetInstance();
    private readonly AutomataAlgorithmWorkspace automataAlgorithmWorkspace =
        AutomataAlgorithmWorkspace.GetInstance();

    public MainWindow()
    {
        InitializeComponent();
        ConfigureImagesDirectory();
        Visualization.RenderTransform = st;
    }
   
    private static void ConfigureImagesDirectory()
    {
        if (!Directory.Exists("./images"))
        {
            Directory.CreateDirectory("./images");
        }
        else
        {
            ClearDirectory("./images");
        }
    }

    private static void ClearDirectory(string path)
    {
        foreach (var file in Directory.GetFiles(path))
        {
            File.Delete($"./{file}");
        }
    }

    private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var automata = GetAutomata();
            var selectedAlgorithmName = Algolist.SelectionBoxItem.ToString();
            if (selectedAlgorithmName == AlgorithmResolver.GetService<MinimizationAlgorithm>().Name)
            {
                if (automata is DFA dfa)
                {
                    var algorithm = AlgorithmResolver.GetService<MinimizationAlgorithm>();
                    var transformed = algorithm.Get(dfa);
                    automataAlgorithmWorkspace.AddContent(transformed);
                }
                else
                {
                    MessageBox.Show("Автомат не является ДКА");
                }
            }
            else if (selectedAlgorithmName == AlgorithmResolver.GetService<AcceptWordAlgorithm>().Name)
            {
                var algorithm = AlgorithmResolver.GetService<AcceptWordAlgorithm>();
                var word = inputWordWorkspace.Word.Text;
                var answer = algorithm.Get(automata, word) ? "распознаёт" : "не распознаёт";
                inputWordWorkspace.AddContent(answer);
            }
            else if (selectedAlgorithmName == AlgorithmResolver.GetService<DeterminizationAlgorithm>().Name)
            {
                if (automata is NDFA ndfa)
                {
                    var algorithm = AlgorithmResolver.GetService<DeterminizationAlgorithm>();
                    var transformed = algorithm.Get(ndfa);
                    automataAlgorithmWorkspace.AddContent(transformed);
                }
                else
                {
                    MessageBox.Show("Автомат не является НКА");
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Во время работы произошла ошибка: {exception.Message}");
        }
    }

    private void Algolist_OnDropDownClosed(object? sender, EventArgs e)
    {
        AnswerField.Children.Clear();
        ApplyButton.IsEnabled = true;
        var selectedAlgorithmName = Algolist.SelectionBoxItem.ToString();
        if (selectedAlgorithmName == AlgorithmResolver.GetService<AcceptWordAlgorithm>().Name)
        {
            inputWordWorkspace.Init(AnswerField);
        }
        else if (selectedAlgorithmName == AlgorithmResolver.GetService<MinimizationAlgorithm>().Name || 
                 selectedAlgorithmName == AlgorithmResolver.GetService<DeterminizationAlgorithm>().Name)
        {
            automataAlgorithmWorkspace.Init(AnswerField);
        }
    }

    private Automata GetAutomata()
    {
        return AutomataParser.GetAutomata(StartState.Text, TerminateStates.Text, TableInput.Text);
    }

    private void VisualButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var automata = GetAutomata();
            GenerateImage(automata);
            var imagePath = Directory.GetCurrentDirectory() + $"/images/{automata.Id}.png";
            var uri = new Uri(imagePath);
            Visualization.Source = new BitmapImage(uri);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void GenerateImage(Automata automata)
    {
        using (var writer = new StreamWriter("temp.dot"))
        {
            writer.Write(ConvertAutomataToDotFormat.Convert(automata));
        }
        var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "./bin/dot.exe",
            UseShellExecute = false,
            Arguments = $"temp.dot -Tpng -o ./images/{automata.Id}.png"
        };
        process.Start();
        process.WaitForExit();
        File.Delete("temp.dot");
    }

    private void Pattern_OnClick(object sender, RoutedEventArgs e)
    {
        var output = new StringBuilder();
        for (var state = 1; state <= 5; state++)
        {
            for (var letter = 97; letter < 99; letter++)
            {
                output.Append($"{state}.{(char) letter} = ");
            }

            output.Append('\n');
        }

        TableInput.Text += output.ToString();
    }

    private void RandomNDFA_OnClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = NDFA.GetRandom(random.Next(3, 6), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTextForm();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }
    
    private void RandomDFA_OnClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = DFA.GetRandom(random.Next(3, 10), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTextForm();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }

    private void Update(object sender, RoutedEventArgs e)
    {
        var process = new Process();
        var url = "https://github.com/sadovnichek/AutomataViz/releases/download/v1.0/AutomataViz.zip";
        var pathToSave = Environment.CurrentDirectory;
        var appName = $"{AppDomain.CurrentDomain.FriendlyName}.exe";
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "Updater.exe",
            Arguments = $"{url} {pathToSave} {appName}"
        };
        process.Start();
        Environment.Exit(0);
    }
    
    private void image_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var zoom = e.Delta > 0 ? 0.1 : -0.05;
        st.ScaleX += zoom;
        st.ScaleY += zoom;
        st.CenterX = e.MouseDevice.GetPosition(Visualization).X;
        st.CenterY = e.MouseDevice.GetPosition(Visualization).Y;
    }

    private void CreateTask_OnClick(object sender, RoutedEventArgs e)
    {
        var taskWindow = new TaskWindow();
        taskWindow.Show();
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var algorithm in AlgorithmResolver.GetAllServices())
        {
            Algolist.Items.Add(algorithm.Name);
        }
    }
}