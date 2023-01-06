using System.Windows.Controls;

namespace AutomataUI.Workspaces;

/*Singleton*/
public class AcceptWordWorkspace
{
    private static AcceptWordWorkspace _instance;
    public Label Label { get; private set; }
    public TextBox Word { get; private set;}
    public Label AnswerLabel { get; private set;}

    private AcceptWordWorkspace() { }

    public static AcceptWordWorkspace GetInstance()
    {
        if (_instance == null)
        {
            _instance = new AcceptWordWorkspace();
        }
        return _instance;
    }
    
    public void Init(StackPanel answerField)
    {
        Label = new Label
        {
            FontSize = 18,
            Content = "Введите слово:"
        };
        Word = new TextBox
        {
            FontSize = 18,
        };
        AnswerLabel = new Label
        {
            FontSize = 18,
            Content = "Ответ: "
        };
        
        answerField.Children.Add(Label);
        answerField.Children.Add(Word);
        answerField.Children.Add(AnswerLabel);
    }

    public void AddContent(string content)
    {
        AnswerLabel.Content = "Ответ: " + content;
    }
}