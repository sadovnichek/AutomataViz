using Domain.Automatas;

namespace Domain.Algorithm;

public interface IAlgorithm : IService
{
    string Name { get; }
}

public interface IAlgorithmTransformer : IAlgorithm
{
    Automata Get(Automata automata);
}

public interface IAlgorithmRecognizer : IAlgorithm
{
    bool Get(Automata automata, string word);
}