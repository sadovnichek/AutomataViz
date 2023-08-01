using System.Windows;
using System.Windows.Controls;
using Domain.Automatas;

namespace AutomataUI.Workspaces;

public class AutomataWorkspace : IAutomataWorkspace
{
    private Label answerLabel;
    private TextBox transitionTable;
    private Label startStateLabel;
    private TextBox startStateText;
    private Label terminateStatesLabel;
    private TextBox terminateStatesText;

    public void Init(StackPanel parent)
    {
        answerLabel = new Label
        {
            FontSize = 18,
            Content = "Ответ:"
        };
        transitionTable = new TextBox
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
        startStateLabel = new Label
        {
            FontSize = 18,
            Content = "Начальное состояние"
        };
        startStateText = new TextBox
        {
            FontSize = 18,
            IsReadOnly = true,
        };
        terminateStatesLabel = new Label
        {
            FontSize = 18,
            Content = "Конечные состояния"
        }; 
        terminateStatesText = new TextBox
        {
            FontSize = 18,
            IsReadOnly = true,
        };
        parent.Children.Add(answerLabel);
        parent.Children.Add(transitionTable);
        parent.Children.Add(startStateLabel);
        parent.Children.Add(startStateText);
        parent.Children.Add(terminateStatesLabel);
        parent.Children.Add(terminateStatesText);
    }

    public void AddContent(Automata transformed)
    {
        transitionTable.Text = transformed.GetTextForm();
        startStateText.Text = transformed.StartState;
        terminateStatesText.Text = string.Join(" ", transformed.TerminateStates);
    }
}