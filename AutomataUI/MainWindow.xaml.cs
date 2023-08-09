﻿using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Domain.Algorithm;
using Domain.Automatas;
using AutomataUI.Workspaces;
using Application;
using System.Linq;
using Infrastructure;

namespace AutomataUI;

public partial class MainWindow
{
    private readonly ScaleTransform st = new();
    private readonly IServiceResolver serviceResolver;

    public MainWindow(IServiceResolver serviceResolver)
    {
        this.serviceResolver = serviceResolver;
        InitializeComponent();
        ConfigureImagesDirectory();
        Visualization.RenderTransform = st;
    }
   
    private void ConfigureImagesDirectory()
    {
        if (!Directory.Exists("./images"))
            Directory.CreateDirectory("./images");
        else
            ClearDirectory("./images");
    }

    private void ClearDirectory(string path)
    {
        foreach (var file in Directory.GetFiles(path))
        {
            File.Delete($"./{file}");
        }
    }

    private void ImplementAlgorithmOnButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var automata = GetAutomata();
            var selectedAlgorithmName = Algorithms.SelectionBoxItem.ToString();
            var service = serviceResolver.GetAlgorithmByName(selectedAlgorithmName);
            switch (service)
            {
                case IAlgorithmTransformer transformer:
                    ImplementTransformerAlgorithm(automata, transformer);
                    break;
                case IAlgorithmRecognizer recognizer:
                {
                    var word = WorkspaceResolver.GetService<IWordInputWorkspace>().GetInput();
                    ImplementRecognitionAlgorithm(automata, word, recognizer);
                    break;
                }
            }
        }
        catch(IncorrectInputException exception)
        {
            MessageBox.Show($"Ошибка ввода: {exception.Message}");
        }
        catch(InvalidOperationException exception)
        {
            MessageBox.Show($"Ошибка выполнения: {exception.Message}");
        }
    }

    private void ImplementTransformerAlgorithm(Automata automata, IAlgorithmTransformer algorithm)
    {
        var transformed = algorithm.Get(automata);
        WorkspaceResolver.GetService<IAutomataWorkspace>().AddContent(transformed);
    }

    private void ImplementRecognitionAlgorithm(Automata automata, string word, IAlgorithmRecognizer recognizer)
    {
        var answer = recognizer.Get(automata, word) ? "распознаёт" : "не распознаёт";
        WorkspaceResolver.GetService<IWordInputWorkspace>().AddContent(answer);
    }

    private void SelectAlgorithm(object sender, EventArgs e)
    {
        AnswerField.Children.Clear();
        var selectedAlgorithmName = Algorithms.SelectionBoxItem.ToString();
        if (string.IsNullOrEmpty(selectedAlgorithmName))
            return;
        ImplementAlgorithmButton.IsEnabled = true;
        var service = serviceResolver.GetAlgorithmByName(selectedAlgorithmName);
        switch (service)
        {
            case IAlgorithmRecognizer:
                WorkspaceResolver.GetService<IWordInputWorkspace>().Init(AnswerField);
                break;
            case IAlgorithmTransformer:
                WorkspaceResolver.GetService<IAutomataWorkspace>().Init(AnswerField);
                break;
        }
    }

    private Automata GetAutomata()
    {
        return AutomataParser.GetAutomata(StartState.Text, TerminateStates.Text, TableInput.Text);
    }

    private void VisualizeAutomataOnButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var service = serviceResolver.GetService<IVisualizationService>();
            var automata = GetAutomata();
            var uri = service.GetImageUri(automata);
            Visualization.Source = new BitmapImage(uri);
        }
        catch(IncorrectInputException exception)
        {
            MessageBox.Show($"Ошибка ввода: {exception.Message}");
        }
    }

    private void InsertPatternOnButtonClick(object sender, RoutedEventArgs e)
    {
        var output = new StringBuilder();
        for (var state = 1; state <= 5; state++)
        {
            for (var letter = 97; letter < 99; letter++)
            {
                output.Append($"{state}.{(char)letter} = ");
            }
            output.Append('\n');
        }
        TableInput.Text += output.ToString();
    }

    private void GetRandomNDFAOnButtonClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = serviceResolver.GetService<IRandomAutomataService>()
            .GetRandomNDFA(random.Next(3, 6), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTransitionTableFormatted();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }
    
    private void GetRandomDFAOnButtonClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = serviceResolver.GetService<IRandomAutomataService>()
            .GetRandomDFA(random.Next(3, 10), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTransitionTableFormatted();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }
    
    private void ZoomImageOnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var zoom = e.Delta > 0 ? 0.1 : -0.05;
        st.ScaleX += zoom;
        st.ScaleY += zoom;
        st.CenterX = e.MouseDevice.GetPosition(Visualization).X;
        st.CenterY = e.MouseDevice.GetPosition(Visualization).Y;
    }

    private void AddAlgorithms(object sender, RoutedEventArgs e)
    {
        serviceResolver.GetAllAlgorithms()
            .ToList()
            .ForEach(algorithm => Algorithms.Items.Add(algorithm.Name));
    }
}