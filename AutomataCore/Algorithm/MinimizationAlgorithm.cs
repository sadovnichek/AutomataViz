﻿using Automata.Infrastructure;

namespace Automata.Algorithm;

/*Singleton*/
public class MinimizationAlgorithm : IAlgorithm
{
    private static MinimizationAlgorithm? instance;
    private DFA dfa;
    public string Name => "Алгоритм минимизации ДКА";
    public bool IsTaskable => true;
    
    private MinimizationAlgorithm() { }

    public static MinimizationAlgorithm GetInstance()
    {
        if (instance == null)
            instance = new MinimizationAlgorithm();
        return instance;
    }
    
    public DFA Get(DFA source)
    {
        dfa = source.ExceptStates(source.GetUnreachableStates());
        var classes = GetClasses();
        var transitions = new HashSet<Tuple<string, string, string>>();
        var start = GetSet(dfa.StartState, classes);
        var terminates = dfa.TerminateStates.Select(v => GetSet(v, classes)).Select(x => x.SetToString())
            .ToHashSet();
        foreach (var cls in classes)
        {
            var firstElement = cls.First();
            foreach (var letter in dfa.Alphabet)
            {
                var value = GetSet(dfa[firstElement, letter], classes).SetToString();
                transitions.Add(Tuple.Create(cls.SetToString(), letter, value));
            }
        }

        return new DFA(classes.Select(x => x.SetToString()).ToHashSet(), 
            dfa.Alphabet, transitions, start.SetToString(), terminates);
    }
    
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
            foreach (var letter in dfa.Alphabet)
            {
                var result = dfa[element, letter];
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
        var terminates = dfa.TerminateStates;
        var nonTerminateStates = dfa.States.Where(s => !terminates.Contains(s)).ToHashSet();
        var queue = new Queue<HashSet<string>>();
        queue.Enqueue(terminates.ToHashSet()); 
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
}