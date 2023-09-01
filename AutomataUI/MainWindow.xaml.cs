using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Domain.Services;
using Domain.Automatas;
using AutomataUI.Workspaces;
using Application;
using System.Linq;

namespace AutomataUI;

public partial class MainWindow
{
    private readonly ScaleTransform scaleTransform;
    private readonly IVisualizationService visualizationService;
    private readonly IRandomAutomataService randomAutomataService;
    private readonly IAutomataParser automataParser;
    private readonly IAutomataWorkspace automataWorkspace;
    private readonly IWordInputWorkspace wordInputWorkspace;
    private readonly IEnumerable<IAlgorithm> algorithms;

    public MainWindow(IVisualizationService visualizationService,
        IRandomAutomataService randomAutomataService,
        IEnumerable<IAlgorithm> algorithms,
        IAutomataParser automataParser,
        IAutomataWorkspace automataWorkspace,
        IWordInputWorkspace wordInputWorkspace)
    {
        InitializeComponent();
        ConfigureImagesDirectory();
        this.visualizationService = visualizationService;
        this.automataParser = automataParser;
        this.automataWorkspace = automataWorkspace;
        this.wordInputWorkspace = wordInputWorkspace;
        this.randomAutomataService = randomAutomataService;
        this.algorithms = algorithms;
        scaleTransform = new ScaleTransform();
        Visualization.RenderTransform = scaleTransform;
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
            var service = algorithms.First(x => x.Name == selectedAlgorithmName);
            switch (service)
            {
                case IAlgorithmTransformer transformer:
                {
                    ImplementTransformerAlgorithm(automata, transformer);
                    break;
                }
                case IAlgorithmRecognizer recognizer:
                {
                    var word = wordInputWorkspace.GetInput();
                    ImplementRecognitionAlgorithm(automata, word, recognizer);
                    break;
                }
            }
        }
        catch(ArgumentException exception)
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
        automataWorkspace.AddContent(transformed);
    }

    private void ImplementRecognitionAlgorithm(Automata automata, string word, IAlgorithmRecognizer recognizer)
    {
        var answer = recognizer.Get(automata, word) 
            ? "распознаёт" 
            : "не распознаёт";
        wordInputWorkspace.AddContent(answer);
    }

    private void SelectAlgorithm(object sender, EventArgs e)
    {
        AnswerField.Children.Clear();
        var selectedAlgorithmName = Algorithms.SelectionBoxItem.ToString();
        if (string.IsNullOrEmpty(selectedAlgorithmName))
            return;
        ImplementAlgorithmButton.IsEnabled = true;
        var service = algorithms.First(x => x.Name == selectedAlgorithmName);
        switch (service)
        {
            case IAlgorithmRecognizer:
                wordInputWorkspace.Init(AnswerField);
                break;
            case IAlgorithmTransformer:
                automataWorkspace.Init(AnswerField);
                break;
        }
    }

    private Automata GetAutomata() =>
        automataParser.GetAutomata(StartState.Text, TerminateStates.Text, TableInput.Text);

    private void VisualizeAutomataOnButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var automata = GetAutomata();
            var imageFilePath = $"{Directory.GetCurrentDirectory()}/images/{automata.Id}.png";
            visualizationService.SaveAutomataImage(automata, imageFilePath);
            Visualization.Source = new BitmapImage(new Uri(imageFilePath));
        }
        catch(ArgumentException exception)
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
        var randomAutomata = randomAutomataService
            .GetRandom(random.Next(3, 6), random.Next(2, 4), false);
        TableInput.Text = randomAutomata.GetTransitionTableFormatted();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }
    
    private void GetRandomDFAOnButtonClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = randomAutomataService
            .GetRandom(random.Next(3, 10), random.Next(2, 4), true);
        TableInput.Text = randomAutomata.GetTransitionTableFormatted();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }
    
    private void ZoomImageOnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var zoom = e.Delta > 0 ? 0.1 : -0.05;
        scaleTransform.ScaleX += zoom;
        scaleTransform.ScaleY += zoom;
        scaleTransform.CenterX = e.MouseDevice.GetPosition(Visualization).X;
        scaleTransform.CenterY = e.MouseDevice.GetPosition(Visualization).Y;
    }

    private void AddAlgorithms(object sender, RoutedEventArgs e)
    {
        algorithms
            .ToList()
            .ForEach(algorithm => Algorithms.Items.Add(algorithm.Name));
    }

    private void InsertLambdaOnButtonClick(object sender, RoutedEventArgs e)
    {
        var caretIndex = TableInput.CaretIndex;
        TableInput.Text = TableInput.Text.Insert(caretIndex, LambdaNDFA.Lambda);
        TableInput.CaretIndex = caretIndex + 1;
    }
}