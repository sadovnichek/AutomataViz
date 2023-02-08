using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AutomataCore.Task;
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
        foreach (var task in TaskResolver.GetAllServices())
        {
            Tasks.Items.Add(task.Name);
        }
    }

    private IAutomataTask GetTask(string taskName)
    {
        return TaskResolver.GetServiceByName(taskName)
            .Configure(description, statesNumber, alphabet);
    }
    
    private Task Create(string taskName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "LaTeX files (*.tex)|*.tex|All files (*.*)|*.*"
        };
        var task = new Task(() =>
        {
            TestPaper.Create(variantsNumber, saveFileDialog.FileName, withSolution)
                .AddTask(GetTask(taskName))
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
            variantsNumber = int.Parse(Number.Text);
            withSolution = WithSolution.IsChecked!.Value;
            Status.Foreground = Brushes.Black;
            Status.Text = "В процессе создания...";
            await Create(Tasks.SelectionBoxItem.ToString());
            Status.Text = "Готово";
            Status.Foreground = Brushes.Green;
            //}
        //catch (Exception exception)
        //{
            //MessageBox.Show(exception.Message);
        //}
    }
}