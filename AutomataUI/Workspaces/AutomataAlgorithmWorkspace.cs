using System.Windows;
using System.Windows.Controls;
using Domain.Automatas;

namespace AutomataUI.Workspaces;

/*Singleton*/
public class AutomataAlgorithmWorkspace
{
    private static AutomataAlgorithmWorkspace? instance;
    private Label AnswerLabel { get; set; }
    private TextBox Transformed { get; set; }
    private Label StartStatesLabel { get; set; }
    private TextBox StartStatesText { get; set; }
    private Label TerminateStatesLabel { get; set; }
    private TextBox TerminateStatesText { get; set; }
    
    private AutomataAlgorithmWorkspace() { }
    
    public static AutomataAlgorithmWorkspace GetInstance()
    {
        if (instance == null)
        {
            instance = new AutomataAlgorithmWorkspace();
        }
        return instance;
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