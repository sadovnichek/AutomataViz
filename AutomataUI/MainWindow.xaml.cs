using System;
using System.Diagnostics;
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

namespace AutomataUI;

public partial class MainWindow
{
    //TODO:
    // Объявление воркспейсов в начале
    private readonly ScaleTransform st = new();

    public MainWindow()
    {
        InitializeComponent();
        ConfigureImagesDirectory();
        Visualization.RenderTransform = st;
    }
   
    private static void ConfigureImagesDirectory()
    {
        if (!Directory.Exists("./images"))
            Directory.CreateDirectory("./images");
        else
            ClearDirectory("./images");
    }

    private static void ClearDirectory(string path)
    {
        foreach (var file in Directory.GetFiles(path))
        {
            File.Delete($"./{file}");
        }
    }

    private void ImplementAlgorithmOnButtonClick(object sender, RoutedEventArgs e)
    {
        var automata = GetAutomata();
        var selectedAlgorithmName = Algorithms.SelectionBoxItem.ToString();
        var service = AlgorithmResolver.GetServiceByName(selectedAlgorithmName);
        if (service is IAlgorithmTransformer transformer)
            ImplementTransformerAlgorithm(automata, transformer);
        else if (service is IAlgorithmRecognizer recognizer)
        {
            var word = WorkspaceResolver.GetService<IWordInputWorkspace>().GetInput();
            ImplementRecognitionAlgorithm(automata, word, recognizer);
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
        ImplementAlgorithmButton.IsEnabled = true;
        var selectedAlgorithmName = Algorithms.SelectionBoxItem.ToString();
        var service = AlgorithmResolver.GetServiceByName(selectedAlgorithmName);
        if (service is IAlgorithmRecognizer)
            WorkspaceResolver.GetService<IWordInputWorkspace>().Init(AnswerField);
        else if (service is IAlgorithmTransformer)
            WorkspaceResolver.GetService<IAutomataWorkspace>().Init(AnswerField);
    }

    private Automata GetAutomata()
    {
        return AutomataParser.GetAutomata(StartState.Text, TerminateStates.Text, TableInput.Text);
    }

    private void VisualizeAutomataOnButtonClick(object sender, RoutedEventArgs e)
    {
        var automata = GetAutomata();
        var fileName = "temp.dot";
        WriteToFile(fileName, automata);
        GenerateImage(fileName, automata);
        var imagePath = Directory.GetCurrentDirectory() + $"/images/{automata.Id}.png"; // нельзя через ./
        var uri = new Uri(imagePath);
        Visualization.Source = new BitmapImage(uri);
        File.Delete(fileName);
    }

    private void WriteToFile(string fileName, Automata automata)
    {
        using var writer = new StreamWriter(fileName);
        writer.Write(ConvertAutomataToDotFormat.Convert(automata));
    }

    private void GenerateImage(string sourceFileName, Automata automata)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "./bin/dot.exe",
                UseShellExecute = false,
                Arguments = $"{sourceFileName} -Tpng -o ./images/{automata.Id}.png"
            }
        };
        process.Start();
        process.WaitForExit();
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
        var randomAutomata = NDFA.GetRandom(random.Next(3, 6), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTextForm();
        StartState.Text = randomAutomata.StartState;
        TerminateStates.Text = string.Join(" ", randomAutomata.TerminateStates);
    }
    
    private void GetRandomDFAOnButtonClick(object sender, RoutedEventArgs e)
    {
        var random = new Random();
        var randomAutomata = DFA.GetRandom(random.Next(3, 10), random.Next(2, 4));
        TableInput.Text = randomAutomata.GetTextForm();
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

    private void ShowTaskWindow(object sender, RoutedEventArgs e)
    {
        var taskWindow = new TaskWindow();
        taskWindow.Show();
    }

    private void AddAlgorithms(object sender, RoutedEventArgs e)
    {
        AlgorithmResolver.GetAllServices()
            .ToList()
            .ForEach(algorithm => Algorithms.Items.Add(algorithm.Name));
    }
}