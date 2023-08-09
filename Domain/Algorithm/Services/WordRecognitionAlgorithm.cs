using Domain.Automatas;

namespace Domain.Algorithm;

public class WordRecognitionAlgorithm : IAlgorithmRecognizer
{
    public string Name => "Проверить слово на распознаваемость";

    public bool Get(Automata automata, string word)
    {
        return automata.IsRecognizeWord(word);
    }
}