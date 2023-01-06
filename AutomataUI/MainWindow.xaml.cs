using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Automata.Algorithm;
using Automata.Infrastructure;
using GraphVizDotNetLib;
using Automata;
using AutomataUI.Workspaces;
using Microsoft.Win32;

namespace AutomataUI;

public partial class MainWindow
{
    private Bitmap _currentDisplayedImage;
    private readonly ScaleTransform _st = new();
    private readonly AcceptWordWorkspace _acceptWordWorkspace = AcceptWordWorkspace.GetInstance();
    private readonly MinimizationAlgorithmWorkspace _minimizationAlgorithmWorkspace =
        MinimizationAlgorithmWorkspace.GetInstance();

    /*Creates a directory to store images*/
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

    public MainWindow()
    {
        InitializeComponent();
        ConfigureImagesDirectory();
        Visualization.RenderTransform = _st;
    }

    /*Creates an automata from user input*/
    private Automata.Automata GetAutomata()
    {
        var states = new HashSet<string>();
        var alphabet = new HashSet<string>();
        var terminates = ParseTerminateStates();
        var start = ParseStartState();
        var transitions = new HashSet<Tuple<string, string, string>>();

        var regex = new Regex(@"(\w+|{(.*?)}|∅).\w+\s*=\s*(\w+|{(.*?)}|∅)");
        var matches = regex.Matches(TableInput.Text);

        foreach (Match match in matches)
        {
            var line = match.Value;
            var key = line.Split("=")[0].Trim();
            var state = key.Split(".")[0];
            var symbol = key.Split(".")[1];
            var value = line.Split("=")[1].Trim();
            states.Add(state);
            alphabet.Add(symbol);
            transitions.Add(Tuple.Create(state, symbol, value));
        }
        
        if(transitions.GroupBy(x => new { x.Item1, x.Item2 }).Any(group => group.Count() > 1))
            return new NDFA(states, alphabet, transitions, start, terminates);
        return new DFA(states, alphabet, transitions, start, terminates);
    }

    /*Actions after apply button pressing*/
    private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var automata = GetAutomata();
            var selectedAlgorithmName = Algolist.SelectionBoxItem.ToString();
            if (selectedAlgorithmName == MinimizationAlgorithm.GetInstance().Name)
            {
                if (automata is DFA dfa)
                {
                    var algorithm = MinimizationAlgorithm.GetInstance();
                    var transformed = algorithm.Get(dfa);
                    _minimizationAlgorithmWorkspace.AddContent(transformed);
                }
                else
                {
                    MessageBox.Show("Автомат не является ДКА");
                }
            }
            else if (selectedAlgorithmName == AcceptWordAlgorithm.GetInstance().Name)
            {
                var algorithm = AcceptWordAlgorithm.GetInstance();
                var word = _acceptWordWorkspace.Word.Text;
                var answer = algorithm.Get(automata, word) ? "распознаёт" : "не распознаёт";
                _acceptWordWorkspace.AddContent(answer);
            }
            else if (selectedAlgorithmName == DeterminizationAlgorithm.GetInstance().Name)//
            {
                if (automata is NDFA ndfa)
                {
                    var algorithm = DeterminizationAlgorithm.GetInstance();//
                    var transformed = algorithm.Get(ndfa);
                    _minimizationAlgorithmWorkspace.AddContent(transformed);
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

    /*Actions after choosing an algorithm from list*/
    private void Algolist_OnDropDownClosed(object? sender, EventArgs e)
    {
        AnswerField.Children.Clear();
        ApplyButton.IsEnabled = true;
        var selectedAlgorithmName = Algolist.SelectionBoxItem.ToString();
        if (selectedAlgorithmName == AcceptWordAlgorithm.GetInstance().Name)
        {
            _acceptWordWorkspace.Init(AnswerField);
        }
        else if (selectedAlgorithmName == MinimizationAlgorithm.GetInstance().Name || 
                 selectedAlgorithmName == DeterminizationAlgorithm.GetInstance().Name)//
        {
            _minimizationAlgorithmWorkspace.Init(AnswerField);
        }
    }

    /*Actions after visual button pressing*/
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

    private string ParseStartState()
    {
        var input = StartState.Text;
        if (input.Length == 0)
        {
            throw new Exception("Поле начальных состояний заполнено не корректно");
        }

        return input.Trim();
    }

    private HashSet<string> ParseTerminateStates()
    {
        var input = TerminateStates.Text;
        if (input.Length == 0)
        {
            throw new Exception("Поле заключительных состояний заполнено не корректно");
        }

        var regex = new Regex(@"(\w+|{(.*?)})");
        var matches = regex.Matches(input);

        return matches.Select(m => m.Value).ToHashSet();
    }

    private void GenerateImage(Automata.Automata automata)
    {
        var dot = automata.ConvertToDotFormat();
        var gv = new GraphVizRenderer("./bin");
        var image = gv.DrawGraphFromDotCode(dot);
        image.Save($"./images/{automata.Id}.png");
        _currentDisplayedImage = image;
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

    private void ImageSave_OnClick(object sender, RoutedEventArgs e)
    {
        if (Visualization.Source == null) return;
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "picture files (*.png)|*.png|All files (*.*)|*.*";
        if (saveFileDialog.ShowDialog() == true)
        {
            _currentDisplayedImage.Save(saveFileDialog.FileName);
        }
    }

    private void RandomNDFA_OnClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = NDFA.GetRandom(random.Next(5, 11), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTextForm();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }
    
    private void RandomDFA_OnClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = DFA.GetRandom(random.Next(5, 11), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTextForm();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }

    private void Update(object sender, RoutedEventArgs e)
    {
        var process = new Process();
        var url = "https://github.com/sadovnichek/AutomataViz/releases/download/v1.0/AutomataViz.zip";
        var pathToSave = Environment.CurrentDirectory;
        var appName = "AutomataUI.exe";
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
        _st.ScaleX += zoom;
        _st.ScaleY += zoom;
        _st.CenterX = e.MouseDevice.GetPosition(Visualization).X;
        _st.CenterY = e.MouseDevice.GetPosition(Visualization).Y;
    }

    private void CreateTask_OnClick(object sender, RoutedEventArgs e)
    {
        var taskWindow = new TaskWindow();
        taskWindow.Show();
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var algorithmName in AlgorithmResolver.Algorithms.Keys)
        {
            Algolist.Items.Add(algorithmName);
        }
    }
}