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

namespace AutomataUI;

public partial class MainWindow
{
    //TODO:
    // Объявление воркспейсов в начале
    // Названия методом в соответствии со стилем
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
            var service = AlgorithmResolver.GetServiceByName(selectedAlgorithmName);
            if (service is IAlgorithmTransformer transformer)
                ImplementTransformerAlgorithm(automata, transformer);
            else if (service is IAlgorithmRecognizer recognizer)
                ImplementRecognitionAlgorithm(automata, inputWordWorkspace.Word.Text, recognizer);
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Во время работы произошла ошибка: {exception.Message}");
        }
    }

    private void ImplementTransformerAlgorithm(Automata automata, IAlgorithmTransformer algorithm)
    {
        var transformed = algorithm.Get(automata);
        automataAlgorithmWorkspace.AddContent(transformed);
    }

    private void ImplementRecognitionAlgorithm(Automata automata, string word, IAlgorithmRecognizer recognizer)
    {
        var answer = recognizer.Get(automata, word) ? "распознаёт" : "не распознаёт";
        inputWordWorkspace.AddContent(answer);
    }

    private void Algolist_OnDropDownClosed(object? sender, EventArgs e)
    {
        AnswerField.Children.Clear();
        ApplyButton.IsEnabled = true;
        var selectedAlgorithmName = Algolist.SelectionBoxItem.ToString();
        var service = AlgorithmResolver.GetServiceByName(selectedAlgorithmName);
        if (service is IAlgorithmRecognizer)
            inputWordWorkspace.Init(AnswerField);
        else if (service is IAlgorithmTransformer)
            automataAlgorithmWorkspace.Init(AnswerField);
    }

    private Automata GetAutomata()
    {
        return AutomataParser.GetAutomata(StartState.Text, TerminateStates.Text, TableInput.Text);
    }

    //exception на все случаи жизни???
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
    
    private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
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