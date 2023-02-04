namespace AutomataCore.Algorithm;

public class AcceptWordAlgorithm : IAlgorithm
{
    public string Name => "Проверить слово на распознаваемость";

    public bool Get(Automata.Automata automata, string word)
    {
        return automata.IsAcceptWord(word);
    }
}