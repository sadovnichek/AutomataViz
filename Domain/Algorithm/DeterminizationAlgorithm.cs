using Domain.Automatas;
using Infrastructure;

namespace Domain.Algorithm;

public class DeterminizationAlgorithm : IAlgorithm
{
    public string Name => "Приведение НКА к ДКА";
    
    public DFA Get(NDFA automata)
    {
        automata = automata.ExceptStates(automata.GetUnreachableStates());
        var states = new HashSet<string>();
        var transitions = new HashSet<Tuple<string, string, string>>();
        var queue = new Queue<string>();
        states.Add(automata.StartState);
        queue.Enqueue(automata.StartState);
        while (queue.Count > 0)
        {
            var from = queue.Dequeue();
            states.Add(from);
            if (from.Contains('{'))
            {
                var elements = from.StringToSet();
                foreach (var symbol in automata.Alphabet)
                {
                    var value = new HashSet<string>();
                    foreach (var element in elements)
                    {
                        value = value.Concat(automata[element, symbol]).ToHashSet();
                    }
                    var to = value.SetToString();
                    transitions.Add(Tuple.Create(from, symbol, to));
                    if (!states.Contains(to))
                    {
                        queue.Enqueue(to);
                    }
                }
            }
            else
            {
                foreach (var symbol in automata.Alphabet)
                {
                    var to = automata[from, symbol].SetToString();
                    transitions.Add(Tuple.Create(from, symbol, to));
                    if (!states.Contains(to))
                    {
                        queue.Enqueue(to);
                    }
                }
            }
        }
        var terminates = states.Select(x => x.StringToSet())
            .Where(s => s.Intersect(automata.TerminateStates).Any())
            .Select(s => s.SetToString())
            .ToHashSet();
        return new DFA(states, automata.Alphabet, transitions, automata.StartState, terminates);
    }
}