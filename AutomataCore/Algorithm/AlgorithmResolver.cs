namespace AutomataCore.Algorithm;

public static class AlgorithmResolver
{
    private static readonly List<IAlgorithm> algorithms = new()
    {
        MinimizationAlgorithm.GetInstance(),
        AcceptWordAlgorithm.GetInstance(),
        DeterminizationAlgorithm.GetInstance()
    };

    public static readonly Dictionary<string, IAlgorithm> Algorithms = algorithms
        .Select(algorithm => new {algorithm.Name, algorithm})
        .ToDictionary(pair => pair.Name, pair => pair.algorithm);
}