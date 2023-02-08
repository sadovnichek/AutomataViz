using System.Windows.Controls;

namespace AutomataUI.Workspaces;

/*Singleton*/
public class InputWordWorkspace
{
    private static InputWordWorkspace _instance;
    private Label Label { get; set; }
    public TextBox Word { get; private set;}
    private Label AnswerLabel { get; set;}

    private InputWordWorkspace() { }

    public static InputWordWorkspace GetInstance()
    {
        if (_instance == null)
        {
            _instance = new InputWordWorkspace();
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