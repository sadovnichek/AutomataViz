using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Automata.Algorithm;
using Automata.Infrastructure;
using GraphVizDotNetLib;
using Automata;
using Microsoft.Win32;

namespace AutomataUI;

public partial class MainWindow
{
    private Bitmap _currentDisplayedImage;
    private readonly ScaleTransform _st = new();

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

    private void CreateTask_OnClick(object sender, RoutedEventArgs e)
    {
        var taskWindow = new TaskWindow();
        taskWindow.Show();
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var algorithm in AlgorithmResolver.GetAll())
        {
            Algolist.Items.Add(algorithm);
        }
    }

    private Automata<string> GetAutomata() // ?
    {
        var table = ParseTableInput();
        var start = ParseStartState();
        var terminates = ParseTerminateStates();
        var states = table.Columns;
        var alphabet = table.Rows;
        return new Automata<string>(table, start, terminates, states, alphabet);
    }

    private static string GetTextForm<T>(Automata<T> automata)
    {
        var output = new StringBuilder();
        foreach (var state in automata.States)
        {
            foreach (var letter in automata.Alphabet)
            {
                output.Append($"{state.SetToString()}.{letter} = {automata.Table[state, letter].SetToString()}");
                output.Append('\t');
            }

            output.Append('\n');
        }

        return output.ToString();
    }

    private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var automata = GetAutomata();
            var algorithm = AlgorithmResolver.ResolveByName(Algolist.SelectionBoxItem.ToString());
            var transformed = algorithm.Get(automata);
            
            TableOutput.Visibility = Visibility.Visible;
            StartStateOutput.Visibility = Visibility.Visible;
            TerminateStatesOutput.Visibility = Visibility.Visible;
            Result.Visibility = Visibility.Visible;
            startStatesLabel.Visibility = Visibility.Visible;
            terminateStatesLabel.Visibility = Visibility.Visible;
            
            TableOutput.Text = GetTextForm(transformed);
            StartStateOutput.Text = transformed.StartState.SetToString();
            TerminateStatesOutput.Text = string.Join(" ", transformed.TerminateStates.Select(s => s.SetToString()));
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Во время работы произошла ошибка: {exception.Message}");
        }
    }

    private void Algorithms_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyButton.IsEnabled = true;
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

    private Table<string, string, string> ParseTableInput()
    {
        var table = new Table<string, string, string>();
        var regex = (TableInput.Text.Contains('{'))
            ? new Regex(@".*?.\w*\s*=\s*{.*?}")
            : new Regex(@"\d*.\w*\s*=\s*\d*");
        var matches = regex.Matches(TableInput.Text);
        foreach (Match match in matches)
        {
            var line = match.Value;
            var key = line.Split("=")[0].Trim();
            var columnIndex = key.Split(".")[0];
            var rowIndex = key.Split(".")[1];
            var value = line.Split("=")[1].Trim();
            table[columnIndex, rowIndex] = value;
        }

        return table;
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
            throw new Exception("Поле начальных состояний заполнено не корректно");
        }

        var regex = (input.Contains('{'))
            ? new Regex(@"{.*?}")
            : new Regex(@"\d+");
        var matches = regex.Matches(input);

        return matches.Select(m => m.Value).ToHashSet();
    }

    private void GenerateImage<T>(Automata<T> transformed)
    {
        var dot = transformed.ConvertToDotFormat();
        var gv = new GraphVizRenderer("./bin");
        var image = gv.DrawGraphFromDotCode(dot);
        image.Save($"./images/{transformed.Id}.png");
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

    private void RandomAutomata_OnClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = Automata<string>.GetRandom(random.Next(5, 11), random.Next(2, 4));
        TableInput.Text = GetTextForm(randomAutomata);
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }

    private void Update(object sender, RoutedEventArgs e)
    {
        var process = new Process();
        var url = "https://github.com/sadovnichek/AutomataViz/releases/download/v1.0/AutomataViz.zip";
        var pathToSave = Environment.CurrentDirectory;
        var appName = "AutomataUI.exe";
        process.StartInfo = new ProcessStartInfo()
        {
            FileName = "Updater.exe",
            Arguments = $"{url} {pathToSave} {appName}"
        };
        process.Start();
        Environment.Exit(0);
    }

    private void AddLambda(object sender, RoutedEventArgs e)
    {
        TableInput.Text += "λ";
    }
    
    private void image_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var zoom = e.Delta > 0 ? 0.1 : -0.05;
        _st.ScaleX += zoom;
        _st.ScaleY += zoom;
        _st.CenterX = e.MouseDevice.GetPosition(Visualization).X;
        _st.CenterY = e.MouseDevice.GetPosition(Visualization).Y;
    }
}