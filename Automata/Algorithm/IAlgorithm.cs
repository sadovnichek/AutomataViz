using Automata.Domain;

namespace Automata.Algorithm;

public interface IAlgorithm
{
    Automata<HashSet<string>> Get(Automata<string> source);
}