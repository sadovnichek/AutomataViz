namespace Automata.Algorithm;

/*Singleton*/
public class AcceptWordAlgorithm : IAlgorithm
{
    private static AcceptWordAlgorithm? instance;
    public string Name => "Проверить слово на распознаваемость";
    public bool IsTaskable => false;

    private AcceptWordAlgorithm() { }

    public static AcceptWordAlgorithm GetInstance()
    {
        if (instance == null)
            instance = new AcceptWordAlgorithm();
        return instance;
    }
    
    public bool Get(Automata automata, string word)
    {
        return automata.IsAcceptWord(word);
    }
}