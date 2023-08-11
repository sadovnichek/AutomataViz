﻿using Domain.Automatas;
using Infrastructure;

namespace Domain.Algorithm;

public class DeterminizationAlgorithm : IAlgorithmTransformer
{
    public string Name => "Приведение НКА к ДКА";

    public Automata Get(Automata automata)
    {
        if (automata is not NDFA)
            throw new InvalidOperationException("Автомат должен быть недетерминированным");
        var pure = (NDFA)automata.ExceptStates(automata.GetUnreachableStates());
        var states = new HashSet<string>();
        var transitions = new HashSet<Tuple<string, string, string>>();
        var queue = new Queue<string>();
        states.Add(pure.StartState);
        queue.Enqueue(pure.StartState);
        while (queue.Count > 0)
        {
            var from = queue.Dequeue();
            states.Add(from);
            if (from.Contains('{'))
            {
                var elements = from.StringToSet();
                foreach (var symbol in pure.Alphabet)
                {
                    var value = new HashSet<string>();
                    foreach (var element in elements)
                    {
                        value = value.Concat(pure[element, symbol]).ToHashSet();
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
                foreach (var symbol in pure.Alphabet)
                {
                    var to = pure[from, symbol].SetToString();
                    transitions.Add(Tuple.Create(from, symbol, to));
                    if (!states.Contains(to))
                    {
                        queue.Enqueue(to);
                    }
                }
            }
        }
        var terminates = states.Select(x => x.StringToSet())
            .Where(s => s.Intersect(pure.TerminateStates).Any())
            .Select(s => s.SetToString())
            .ToHashSet();
        return new DFA(states, pure.Alphabet, transitions, pure.StartState, terminates);
    }
}