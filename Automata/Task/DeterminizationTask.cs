using Automata.Algorithm;
using Automata.Infrastructure;

namespace Automata.Task;

public class DeterminizationTask : ITask
{
    private readonly string description;
    private readonly DeterminizationAlgorithm algorithm;
    private readonly HashSet<string> states;
    private readonly HashSet<string> alphabet;
    
    public DeterminizationTask(string description, int states, HashSet<string> alphabet)
    {
        this.description = description;
        algorithm = DeterminizationAlgorithm.GetInstance();
        this.states = Enumerable.Range(1, states).Select(x => x.ToString()).ToHashSet();
        this.alphabet = alphabet;
    }
    
    private static void WriteBoth(TexFile student, TexFile teacher, string text)
    {
        student.Write(text); 
        teacher.Write(text);
    }

    private static bool IsAppropriate(Automata source, Automata result)
    {
        return result.TerminateStates.Count >= 1
               && result.States.Count > Math.Pow(2, source.States.Count - 1)
               && result.CountCompoundSets() >= 2;
    }
    
    public void Create(TexFile student, TexFile teacher)
    {
        WriteBoth(student, teacher, description);

        var randomAutomata = NDFA.GetRandom(states, alphabet);
        var transformed = algorithm.Get(randomAutomata);

        while (!IsAppropriate(randomAutomata, transformed))
        {
            randomAutomata = NDFA.GetRandom(states, alphabet);
            transformed = algorithm.Get(randomAutomata);
        }
        WriteBoth(student, teacher, randomAutomata.ConvertToTexFormat());
        
        teacher.WriteWhiteSpace(2);
        teacher.Write("Ответ:");
        teacher.Write(transformed.ConvertToTexFormat());
    }
}