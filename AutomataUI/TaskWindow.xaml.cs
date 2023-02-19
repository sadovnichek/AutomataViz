using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AutomataCore.Test;
using Microsoft.Win32;

namespace AutomataUI;

public partial class TaskWindow
{
    private int statesNumber;
    private HashSet<string> alphabet;
    private string description;
    private int studentsAmount;
    private string filename;
    private string taskName;

    public TaskWindow()
    {
        InitializeComponent();
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var task in TaskResolver.GetAllServices())
        {
            Tasks.Items.Add(task.Name);
        }
    }

    private void CreateTestPaper()
    {
        var task = TaskResolver.GetServiceByName(taskName).Configure(description, statesNumber, alphabet);
        TestPaper.Create(studentsAmount, filename)
            .AddTask(task)
            .Generate();
    }

    private void SaveFile()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "LaTeX files (*.tex)|*.tex|All files (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            filename = saveFileDialog.FileName;
        }
    }

    private void FillData()
    {
        if (Tasks.SelectedItem == null)
        {
            throw new Exception("Выберите тип задания");
        }
        statesNumber = int.Parse(StatesNumber.Text);
        alphabet = Alphabet.Text.Trim().Split().ToHashSet();
        description = Description.Text;
        studentsAmount = int.Parse(Number.Text);
        taskName = Tasks.SelectionBoxItem.ToString();
    }

    private async void CreateButtonOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            FillData();
            SaveFile();
            Status.Foreground = Brushes.Black;
            Status.Text = "В процессе создания...";
            await Task.Run(CreateTestPaper);
            Status.Text = "Готово";
            Status.Foreground = Brushes.Green;
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}