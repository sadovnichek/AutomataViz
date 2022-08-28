namespace Automata.Algorithm;

public interface IAlgorithm
{
    Automata<HashSet<string>> Get(Automata<string> source);

    bool IsAppropriate(Automata<string> source, Automata<HashSet<string>> result);
}