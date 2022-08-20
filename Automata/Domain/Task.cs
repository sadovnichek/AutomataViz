using Automata.Algorithm;
using Automata.Infrastructure;

namespace Automata.Domain;

public class Task
{
    private readonly string _description;
    private readonly IAlgorithm _algorithm;
    private readonly int _statesNumber;
    private readonly HashSet<string> _alphabet;

    public Task(string description,
        IAlgorithm algorithm,
        int statesNumber,
        HashSet<string> alphabet)
    {
        _description = description;
        _algorithm = algorithm;
        _statesNumber = statesNumber;
        _alphabet = alphabet;
    }
    
    public void CreateTask(TexFile student, TexFile teacher)
    {
        student.Write(_description);
        teacher.Write(_description);
        var states = Enumerable.Range(1, _statesNumber).Select(x => x.ToString()).ToHashSet();
        var randomAutomata = Automata<string>.GetRandom(states, _alphabet);
        var transformed = _algorithm.Get(randomAutomata);

        while (!(randomAutomata.GetUnreachableStates().Count <= 1 
                 && transformed.CountCompoundSets() >= 2 
                 && transformed.TerminateStates.Count > 1))
        {
            randomAutomata = Automata<string>.GetRandom(states, _alphabet);
            transformed = _algorithm.Get(randomAutomata);
        }
        
        student.Write(randomAutomata.ConvertToTexFormat());
        teacher.Write(randomAutomata.ConvertToTexFormat());
        teacher.WriteWhiteSpace(2);
        teacher.Write("Ответ:");
        teacher.Write(transformed.ConvertToTexFormat());
        student.Write("");
        teacher.Write("");
    }
}