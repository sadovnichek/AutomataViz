﻿using System;
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
    private int statesNumber;
    private HashSet<string> alphabet = null!;
    private string description = null!;
    private int variantsNumber;
    private bool withSolution;

    public TaskWindow()
    {
        InitializeComponent();
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var algorithm in AlgorithmResolver.Algorithms
                     .Where(pair => pair.Value.IsTaskable)
                     .Select(pair => pair.Key))
        {
            Algolist.Items.Add(algorithm);
        }
    }

    private ITask GetTask(string taskName)
    {
        if (taskName == MinimizationAlgorithm.GetInstance().Name)
            return new MinimizationTask(description, statesNumber, alphabet);
        return new DeterminizationTask(description, statesNumber, alphabet);
    }
    
    private void Create()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "LaTeX files (*.tex)|*.tex|All files (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            TestPaper.Create(variantsNumber, saveFileDialog.FileName, withSolution)
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
            statesNumber = int.Parse(StatesNumber.Text);
            alphabet = Alphabet.Text.Replace(',', ' ')
                .Split()
                .Where(x => x.Length > 0)
                .ToHashSet();
            if (Algolist.SelectedItem == null)
            {
                throw new Exception("Выберите тип задания");
            }
            description = Description.Text;
            variantsNumber = int.Parse(Number.Text);
            withSolution = WithSolution.IsChecked!.Value;
            Status.Text = "В процессе создания...";
            Create();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}