using Automata.Algorithm;
using Automata.Infrastructure;

namespace Automata.Task;

public class MinimizationTask : ITask
{
    private string _description;
    private readonly MinimizationAlgorithm _algorithm;
    private readonly int _states;
    private readonly HashSet<string> _alphabet;
    
    public MinimizationTask(string description, int states, HashSet<string> alphabet)
    {
        _description = description;
        _algorithm = MinimizationAlgorithm.GetInstance();
        _states = states;
        _alphabet = alphabet;
    }

    private static void WriteBoth(TexFile student, TexFile teacher, string text)
    {
        student.Write(text); 
        teacher.Write(text);
    }
    
    public bool IsAppropriate(DFA source, DFA result)
    {
        return result.CountCompoundSets() > source.States.Count / 4
               && result.TerminateStates.Count > 1;
    }
    
    public void Create(TexFile student, TexFile teacher)
    {
        WriteBoth(student, teacher, _description);
        var states = Enumerable.Range(1, _states).Select(x => x.ToString()).ToHashSet();
        var randomAutomata = DFA.GetRandom(states, _alphabet);
        var transformed = _algorithm.Get(randomAutomata);

        while (!IsAppropriate(randomAutomata, transformed))
        {
            randomAutomata = DFA.GetRandom(states, _alphabet);
            transformed = _algorithm.Get(randomAutomata);
        }
        WriteBoth(student, teacher, randomAutomata.ConvertToTexFormat());
        
        teacher.WriteWhiteSpace(2);
        teacher.Write("Ответ:");
        teacher.Write(transformed.ConvertToTexFormat());
    }
}