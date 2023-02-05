using AutomataCore.Algorithm;
using AutomataCore.Automata;
using Infrastructure;

namespace AutomataCore.Task;

public class DeterminizationTask : IAutomataTask
{
    private string description;
    private DeterminizationAlgorithm algorithm;
    private HashSet<string> states;
    private HashSet<string> alphabet;
    public string Name { get; }
    
    public DeterminizationTask()
    {
        algorithm = AlgorithmResolver.GetService<DeterminizationAlgorithm>();
        Name = algorithm.Name;
    }

    public IAutomataTask Configure(string description, int states, HashSet<string> alphabet)
    {
        this.description = description;
        this.states = Enumerable.Range(1, states).Select(x => x.ToString()).ToHashSet();
        this.alphabet = alphabet;
        return this;
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
    
    private static void WriteBoth(TexFile student, TexFile teacher, string text)
    {
        student.Write(text); 
        teacher.Write(text);
    }

    private static bool IsAppropriate(Automata.Automata source, Automata.Automata result)
    {
        return result.TerminateStates.Count >= 1
               && result.States.Count >= Math.Pow(2, source.States.Count - 1)
               && result.CountCompoundSets() >= 2;
    }
}