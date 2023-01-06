using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Automata.Algorithm;
using Automata.Task;
using Microsoft.Win32;

namespace AutomataUI;

public partial class TaskWindow
{
    //private readonly BackgroundWorker _worker = new();
    private int _statesNumber;
    private HashSet<string> _alphabet = null!;
    private string _description = null!;
    private int _variantsNumber;
    private bool _withSolution;

    public TaskWindow()
    {
        InitializeComponent();
        //_worker.DoWork += Create!;
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var algorithm in AlgorithmResolver.Algorithms
                     .Where(pair => pair.Value.IsTask)
                     .Select(pair => pair.Key))
        {
            Algolist.Items.Add(algorithm);
        }
    }

    private ITask GetTask(string taskName)
    {
        if (taskName == MinimizationAlgorithm.GetInstance().Name)
            return new MinimizationTask(_description, _statesNumber, _alphabet);
        return new DeterminizationTask(_description, _statesNumber, _alphabet);
    }
    
    private void Create(/*object sender, DoWorkEventArgs e*/)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "LaTeX files (*.tex)|*.tex|All files (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            TestPaper.Create(_variantsNumber, saveFileDialog.FileName, _withSolution)
                .AddTask(GetTask(Algolist.SelectionBoxItem.ToString()))
                .Generate();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Status.Text = "Готово";
                Status.Foreground = Brushes.Green;
            });
        }
    }
    
    private void Create_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _statesNumber = int.Parse(StatesNumber.Text);
            _alphabet = Alphabet.Text.Replace(',', ' ')
                .Split()
                .Where(x => x.Length > 0)
                .ToHashSet();
            if (Algolist.SelectedItem == null)
            {
                throw new Exception("Выберите тип задания");
            }
            _description = Description.Text;
            _variantsNumber = int.Parse(Number.Text);
            _withSolution = WithSolution.IsChecked!.Value;
            Status.Text = "В процессе создания...";
            //_worker.RunWorkerAsync();
            Create();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}