using Automata.Algorithm;
using Automata.Infrastructure;

namespace Automata.Task;

public class AutomataTask : ITask
{
    public string Description { get; }
    private readonly IAlgorithm _algorithm;
    private readonly int _states;
    private readonly HashSet<string> _alphabet;

    public AutomataTask(string description, IAlgorithm algorithm, int states, HashSet<string> alphabet)
    {
        Description = description;
        _algorithm = algorithm;
        _states = states;
        _alphabet = alphabet;
    }

    private static void WriteBoth(TexFile student, TexFile teacher, string text)
    {
        student.Write(text); 
        teacher.Write(text);
    }
    
    public void Create(TexFile student, TexFile teacher)
    {
        WriteBoth(student, teacher, Description);
        var states = Enumerable.Range(1, _states).Select(x => x.ToString()).ToHashSet();
        var randomAutomata = Automata<string>.GetRandom(states, _alphabet);
        var transformed = _algorithm.Get(randomAutomata);

        while (!_algorithm.IsAppropriate(randomAutomata, transformed))
        {
            randomAutomata = Automata<string>.GetRandom(states, _alphabet);
            transformed = _algorithm.Get(randomAutomata);
        }
        WriteBoth(student, teacher, randomAutomata.ConvertToTexFormat());
        
        teacher.WriteWhiteSpace(2);
        teacher.Write("Ответ:");
        teacher.Write(transformed.ConvertToTexFormat());
    }
}