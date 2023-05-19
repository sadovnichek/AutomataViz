using Domain.Automatas;

namespace Domain.Algorithm;

public class AcceptWordAlgorithm : IAlgorithm
{
    public string Name => "Проверить слово на распознаваемость";

    public bool Get(Automata automata, string word)
    {
        return automata.IsAcceptWord(word);
    }
}