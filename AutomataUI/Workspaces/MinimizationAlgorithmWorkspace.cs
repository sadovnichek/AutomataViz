using System.Windows;
using System.Windows.Controls;
using Automata;

namespace AutomataUI.Workspaces;

/*Singleton*/
public class MinimizationAlgorithmWorkspace
{
    private static MinimizationAlgorithmWorkspace? _instance;
    public Label AnswerLabel { get; private set; }
    public TextBox Transformed { get; private set; }
    public Label StartStatesLabel { get; private set; }
    public TextBox StartStatesText { get; private set; }
    public Label TerminateStatesLabel { get; private set; }
    public TextBox TerminateStatesText { get; private set; }
    
    private MinimizationAlgorithmWorkspace() { }
    
    public static MinimizationAlgorithmWorkspace GetInstance()
    {
        if (_instance == null)
        {
            _instance = new MinimizationAlgorithmWorkspace();
        }
        return _instance;
    }

    public void Init(StackPanel panel)
    {
        AnswerLabel = new Label
        {
            FontSize = 18,
            Content = "Ответ:"
        };
        Transformed = new TextBox
        {
            AcceptsTab = true,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            MinHeight = 150, MaxHeight = 150,
            FontSize = 16,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            IsReadOnly = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
        };
        StartStatesLabel = new Label
        {
            FontSize = 18,
            Content = "Начальное состояние"
        };
        StartStatesText = new TextBox
        {
            FontSize = 18,
            IsReadOnly = true,
        };
        TerminateStatesLabel = new Label
        {
            FontSize = 18,
            Content = "Конечные состояния"
        }; 
        TerminateStatesText = new TextBox
        {
            FontSize = 18,
            IsReadOnly = true,
        };
        panel.Children.Add(AnswerLabel);
        panel.Children.Add(Transformed);
        panel.Children.Add(StartStatesLabel);
        panel.Children.Add(StartStatesText);
        panel.Children.Add(TerminateStatesLabel);
        panel.Children.Add(TerminateStatesText);
    }

    public void AddContent(DFA transformed)
    {
        Transformed.Text = transformed.GetTextForm();
        StartStatesText.Text = transformed.StartState;
        TerminateStatesText.Text = string.Join(" ", transformed.TerminateStates);
    }
}