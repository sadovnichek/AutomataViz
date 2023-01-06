using Automata.Algorithm;
using Automata.Infrastructure;

namespace Automata.Task;

public class DeterminizationTask : ITask
{
    private string _description;
    private readonly DeterminizationAlgorithm _algorithm;
    private readonly int _states;
    private readonly HashSet<string> _alphabet;
    
    public DeterminizationTask(string description, int states, HashSet<string> alphabet)
    {
        _description = description;
        _algorithm = DeterminizationAlgorithm.GetInstance();
        _states = states;
        _alphabet = alphabet;
    }
    
    private static void WriteBoth(TexFile student, TexFile teacher, string text)
    {
        student.Write(text); 
        teacher.Write(text);
    }
    
    public bool IsAppropriate(NDFA source, DFA result)
    {
        return result.TerminateStates.Count >= 1
               && result.States.Count > Math.Pow(2, source.States.Count - 1)
               && result.CountCompoundSets() >= 2;
    }
    
    public void Create(TexFile student, TexFile teacher)
    {
        WriteBoth(student, teacher, _description);
        var states = Enumerable.Range(1, _states).Select(x => x.ToString()).ToHashSet();
        var randomAutomata = NDFA.GetRandom(states, _alphabet);
        var transformed = _algorithm.Get(randomAutomata);

        while (!IsAppropriate(randomAutomata, transformed))
        {
            randomAutomata = NDFA.GetRandom(states, _alphabet);
            transformed = _algorithm.Get(randomAutomata);
        }
        WriteBoth(student, teacher, randomAutomata.ConvertToTexFormat());
        
        teacher.WriteWhiteSpace(2);
        teacher.Write("Ответ:");
        teacher.Write(transformed.ConvertToTexFormat());
    }
}