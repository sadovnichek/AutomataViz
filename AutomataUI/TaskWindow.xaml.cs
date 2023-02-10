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
    private HashSet<string> alphabet = null!;
    private string description = null!;
    private int studentsAmount;

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

    private Task Create(string taskName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "LaTeX files (*.tex)|*.tex|All files (*.*)|*.*"
        };
        var task = new Task(() =>
        {
            TestPaper.Create(studentsAmount, saveFileDialog.FileName)
                .AddTask(TaskResolver.GetServiceByName(taskName)
                    .Configure(description, statesNumber, alphabet))
                .Generate();
        });
        if (saveFileDialog.ShowDialog() == true)
        {
            task.Start();
        }
        return task;
    }

    private async void Create_OnClick(object sender, RoutedEventArgs e)
    {
        //try
        //{
        statesNumber = int.Parse(StatesNumber.Text);
        alphabet = Alphabet.Text.Replace(',', ' ')
            .Split()
            .Where(x => x.Length > 0)
            .ToHashSet();
        if (Tasks.SelectedItem == null)
        {
            throw new Exception("Выберите тип задания");
        }

        description = Description.Text;
        studentsAmount = int.Parse(Number.Text);
        Status.Foreground = Brushes.Black;
        Status.Text = "В процессе создания...";
        var selectedTask = Tasks.SelectionBoxItem.ToString();
        if (selectedTask != null)
            await Create(selectedTask);
        Status.Text = "Готово";
        Status.Foreground = Brushes.Green;
        //}
        //catch (Exception exception)
        //{
        //MessageBox.Show(exception.Message);
        //}
    }
}