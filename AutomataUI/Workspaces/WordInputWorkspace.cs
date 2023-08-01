using System.Windows.Controls;

namespace AutomataUI.Workspaces;

public class WordInputWorkspace : IWordInputWorkspace
{
    private Label label;
    public TextBox word;
    private Label answerLabel;
    
    public void Init(StackPanel parent)
    {
        label = new Label
        {
            FontSize = 18,
            Content = "Введите слово:"
        };
        word = new TextBox
        {
            FontSize = 18,
        };
        answerLabel = new Label
        {
            FontSize = 18,
            Content = "Ответ: "
        };
        parent.Children.Add(label);
        parent.Children.Add(word);
        parent.Children.Add(answerLabel);
    }

    public void AddContent(string content)
    {
        answerLabel.Content = "Ответ: " + content;
    }

    public string GetInput()
    {
        return word.Text;
    }
}