using Automata.Domain;

namespace Automata.Algorithm;

public interface IAlgorithm
{
    Automata<HashSet<string>> Get(Automata<string> source);
}

public static class AlgorithmResolver
{
    private static readonly Dictionary<string, IAlgorithm> Algorithms = new()
    {
        { "Алгоритм минимизации ДКА", new MinimizationAlgorithm() },
    };

    public static IAlgorithm ResolveByName(string name)
    {
        return Algorithms[name];
    }

    public static IEnumerable<string> GetAll()
    {
        return Algorithms.Keys;
    }
}