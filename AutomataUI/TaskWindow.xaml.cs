using System.Linq;
using System.Windows;
using Automata.Algorithm;
using Automata.Task;
using Microsoft.Win32;

namespace AutomataUI;

public partial class TaskWindow
{
    public TaskWindow()
    {
        InitializeComponent();
    }

    private void Algorithms_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var algorithm in AlgorithmResolver.GetAll())
        {
            Algolist.Items.Add(algorithm);
        }
    }

    private void Create_OnClick(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog();
        var statesNumber = int.Parse(StatesNumber.Text);
        var alphabet = Alphabet.Text.Replace(',', ' ').Split().Where(x => x.Length > 0).ToHashSet();
        var selectedAlgorithm = AlgorithmResolver.ResolveByName(Algolist.SelectedItem.ToString());
        var description = Description.Text;
        saveFileDialog.Filter = "LaTeX files (*.tex)|*.tex|All files (*.*)|*.*";
        if (saveFileDialog.ShowDialog() == true)
        {
            TestPaper.Create(int.Parse(Number.Text), saveFileDialog.FileName, WithSolution.IsChecked.Value)
                .AddTask(new AutomataTask(description, selectedAlgorithm, statesNumber, alphabet))
                .Generate();
        }
        MessageBox.Show("Готово");
    }
}