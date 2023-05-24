﻿using System.Text;
using Infrastructure;

namespace Domain.Automatas;

public class NDFA : Automata
{
    public NDFA(HashSet<string> states,
        HashSet<string> alphabet,
        string startState,
        HashSet<string> terminateStates)
    {
        Alphabet = alphabet;
        Transitions = new();
        States = states;
        StartState = startState;
        TerminateStates = terminateStates;
    }

    public NDFA(HashSet<string> states,
        HashSet<string> alphabet,
        HashSet<Tuple<string, string, string>> transitions,
        string startState,
        HashSet<string> terminateStates)
    {
        Transitions = transitions;
        Alphabet = alphabet;
        States = states;
        StartState = startState;
        TerminateStates = terminateStates;
    }

    public HashSet<string> this[string state, string symbol]
    {
        get
        {
            return Transitions.Where(x => x.Item1 == state && x.Item2 == symbol)
                .Select(x => x.Item3)
                .ToHashSet();
        }
    }

    public string ConvertToTexFormat()
    {
        var sb = new StringBuilder();
        sb.Append("\n\\begin{tabular}{ c | ");
        sb.AppendJoin(" ", Enumerable.Repeat("c", States.Count));
        sb.Append(" }\n & ");
        foreach (var state in States)
        {
            sb.Append(state.StringToSet().SetToString(true) + (States.Last().Equals(state) ? "" : " & "));
        }
        sb.Append(" \\\\ \n\\hline\n");
        foreach (var symbol in Alphabet)
        {
            sb.Append(symbol + " & ");
            foreach (var state in States)
            {
                sb.Append(this[state, symbol].SetToString(true) + (States.Last().Equals(state) ? "" : " & "));
            }
            sb.Append(" \\\\\n");
        }
        sb.Append("\\end{tabular}\n\\\\\n");
        sb.Append($"вход: {StartState.StringToSet().SetToString(true)}, выходы: {TerminateStates.SetToString(true)}");
        return sb.ToString();
    }

    public NDFA ExceptStates(HashSet<string> exceptedStates)
    {
        var newTerminates = TerminateStates.Where(s => !exceptedStates.Contains(s)).ToHashSet();
        var newStates = States.Where(s => !exceptedStates.Contains(s)).ToHashSet();
        var newAutomata = new NDFA(newStates, Alphabet, StartState, newTerminates);

        foreach (var state in newStates)
        {
            foreach (var symbol in Alphabet)
            {
                var values = this[state, symbol];
                foreach (var value in values)
                {
                    if (!exceptedStates.Contains(value))
                    {
                        newAutomata.AddTransition(state, symbol, value);
                    }
                }
            }
        }

        return newAutomata;
    }

    public override HashSet<string> GetUnreachableStates()
    {
        var start = StartState;
        var used = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(start);

        while (queue.Count != 0)
        {
            var currentState = queue.Dequeue();
            if (used.Contains(currentState))
                continue;
            Alphabet.ToList()
                .ForEach(letter =>
                {
                    var states = this[currentState, letter];
                    foreach (var state in states)
                    {
                        if (!queue.Contains(state))
                            queue.Enqueue(state);
                    }
                });
            used.Add(currentState);
        }

        return States.Where(s => !used.Contains(s)).ToHashSet();
    }
    
    public static NDFA GetRandom(HashSet<string> states, HashSet<string> alphabet)
    {
        var random = new Random();
        var start = states.ToList()[random.Next(0, states.Count)];
        var terminates = states.GetRandomSubset(random.Next(2, states.Count / 2 + 1));
        var randomAutomata = new NDFA(states, alphabet, start, terminates);
        foreach (var state in states)
        {
            foreach (var symbol in alphabet)
            {
                for (int i = 0; i < random.Next(0, 3); i++)
                {
                    var randomIndex = random.Next(0, states.Count);
                    randomAutomata.AddTransition(state, symbol, states.ToList()[randomIndex]);
                }
            }
        }

        return randomAutomata;
    }

    public static NDFA GetRandom(int statesNumber, int alphabetNumber)
    {
        var states = Enumerable.Range(1, statesNumber)
            .Select(x => x.ToString())
            .ToHashSet();
        var alphabet = Enumerable.Range(0, alphabetNumber)
            .Select(x => ((char) (97 + x)).ToString())
            .ToHashSet();
        return GetRandom(states, alphabet);
    }

    public override bool IsAcceptWord(string word)
    {
        var currentNode = new HashSet<string>{StartState};
        foreach (var w in word)
        {
            var resultSet = new HashSet<string>();
            foreach (var q in currentNode)
            {
                resultSet = resultSet.Concat(this[q, w.ToString()]).ToHashSet();
            }
            currentNode = resultSet;
        }
        currentNode.IntersectWith(TerminateStates);
        return currentNode.Count > 0;
    }
}