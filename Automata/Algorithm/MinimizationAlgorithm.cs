using Automata.Domain;
using Automata.Infrastructure;

namespace Automata.Algorithm;

public class MinimizationAlgorithm : IAlgorithm
{
    private Automata<string> _automata;

    private HashSet<string> GetSet(string element, IEnumerable<HashSet<string>> queue)
    {
        return queue.FirstOrDefault(set => set.Contains(element));
    }
    
    private Dictionary<HashSet<HashSet<string>>, HashSet<string>> SplitClass(HashSet<string> set, 
        Queue<HashSet<string>> queue)
    {
        var dict = new Dictionary<HashSet<HashSet<string>>, HashSet<string>>(new SetEqualityComparer());
        foreach (var element in set)
        {
            var characteristicSet = new HashSet<HashSet<string>>();
            foreach (var letter in _automata.Alphabet)
            {
                var result = _automata.Table[element, letter];
                var belongsSet = GetSet(result, queue);
                characteristicSet.Add(belongsSet);
            }
            if (!dict.ContainsKey(characteristicSet))
                dict[characteristicSet] = new HashSet<string>();
            dict[characteristicSet].Add(element);
        }
        return dict;
    }

    private bool CanBeAnyClassSplit(Queue<HashSet<string>> queue)
    {
        return queue.Any(set => SplitClass(set, queue).Count != 1);
    }
    
    private HashSet<HashSet<string>> GetClasses()
    {
        var terminates = _automata.TerminateStates;
        var nonTerminateStates = _automata.States.Where(s => !terminates.Contains(s)).ToHashSet();
        var queue = new Queue<HashSet<string>>();
        queue.Enqueue(terminates); 
        queue.Enqueue(nonTerminateStates);
        while (CanBeAnyClassSplit(queue))
        {
            var set = queue.Peek();
            var classes = SplitClass(set, queue);
            classes.Keys.ToList().ForEach(key => queue.Enqueue(classes[key]));
            queue.Dequeue();
        }
        return queue.ToHashSet();
    }
    
    public Automata<HashSet<string>> Get(Automata<string> source)
    {
        _automata = source.ExceptStates(source.GetUnreachableStates());
        var classes = GetClasses();
        var minAutomataTable = new Table<string, HashSet<string>, HashSet<string>>();
        var start = GetSet(_automata.StartState, classes);
        var terminates = _automata.TerminateStates.Select(v => GetSet(v, classes)).ToHashSet();
        foreach (var cls in classes)
        {
            var firstElement = cls.First();
            foreach (var letter in _automata.Alphabet)
            {
                minAutomataTable[cls, letter] = GetSet(_automata.Table[firstElement, letter], classes);
            }
        }
        return new Automata<HashSet<string>>(minAutomataTable, start, terminates, classes, _automata.Alphabet);
    }
}