using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Automata.Algorithm;
using Automata.Infrastructure;
using Automata.Domain;
using GraphVizDotNetLib;

namespace AutomataUI;

public partial class MainWindow
{
    private void ConfigureImagesDirectory()
    {
        if (!Directory.Exists("./images"))
        {
            Directory.CreateDirectory("./images");
        }
        else
        {
            foreach (var file in Directory.GetFiles("./images"))
            {
                File.Delete($"./{file}");
            }
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        ConfigureImagesDirectory();
    }

    private void CreateTask_OnClick(object sender, RoutedEventArgs e)
    {
        TaskWindow taskWindow = new TaskWindow();
        taskWindow.Show();
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var algorithm in AlgorithmResolver.GetAll())
        {
            Algolist.Items.Add(algorithm);
        }
    }

    private Automata<string> GetAutomata()
    {
        var table = ParseTableInput();
        var start = ParseStartState();
        var terminates = ParseTerminateStates();
        var states = table.Columns;
        var alphabet = table.Rows;
        return new Automata<string>(table, start, terminates, states, alphabet);
    }

    private void WriteOutput<T>(Automata<T> automata)
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

        TableOutput.Text = output.ToString();
        StartStateOutput.Text = automata.StartState.SetToString();
        TerminateStatesOutput.Text = string.Join(" ", automata.TerminateStates.Select(s => s.SetToString()));
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
            WriteOutput(transformed);
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
        gv.DrawGraphFromDotCode(dot).Save($"./images/{transformed.Id}.png");
    }
}