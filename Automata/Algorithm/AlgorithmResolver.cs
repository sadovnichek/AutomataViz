namespace Automata.Algorithm;

public static class AlgorithmResolver
{
    private static List<IAlgorithm> _algorithms = new()
    {
        MinimizationAlgorithm.GetInstance(),
        AcceptWordAlgorithm.GetInstance(),
        DeterminizationAlgorithm.GetInstance()
    };

    public static Dictionary<string, IAlgorithm> Algorithms = _algorithms
        .Select(algorithm => new {algorithm.Name, algorithm})
        .ToDictionary(pair => pair.Name, pair => pair.algorithm);
}