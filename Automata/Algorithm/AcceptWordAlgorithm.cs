namespace Automata.Algorithm;

/*Singleton*/
public class AcceptWordAlgorithm : IAlgorithm
{
    private static AcceptWordAlgorithm? _instance;
    public string Name => "Проверить слово на распознаваемость";
    public bool IsTask => false;

    private AcceptWordAlgorithm() { }

    public static AcceptWordAlgorithm GetInstance()
    {
        if (_instance == null)
            _instance = new AcceptWordAlgorithm();
        return _instance;
    }
    
    public bool Get(Automata automata, string word)
    {
        return automata.IsAcceptWord(word);
    }
}