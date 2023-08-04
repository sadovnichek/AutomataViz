using System.Text.RegularExpressions;
using Domain.Automatas;
using Infrastructure;

namespace Application;

public static class AutomataParser
{
    public static Automata Current { get; set; }
    
    private static readonly Regex regexToReadTerminateStates = new(@"(\w+|{(.*?)})");
    private static readonly Regex regexToReadTransitionTable = new(@"(\w+|{(.*?)}|∅).\w+\s*=\s*(\w+|{(.*?)}|∅)");
    
    public static Automata GetAutomata(string startState, string terminateStates, string transitionTable)
    {
        var states = new HashSet<string>();
        var alphabet = new HashSet<string>();
        var terminates = ParseTerminateStates(terminateStates);
        var start = ParseStartState(startState);
        var transitions = new HashSet<Tuple<string, string, string>>();

        foreach (var (state, symbol, value) in ParseTransitionTable(transitionTable))
        {
            states.Add(state);
            alphabet.Add(symbol);
            transitions.Add(Tuple.Create(state, symbol, value));
        }

        if (transitions.Count == 0)
            throw new IncorrectInputException("Таблица переходов заполнено некорректно");

        if (Automata.IsDfa(transitions, alphabet, states))
            return new DFA(states, alphabet, transitions, start, terminates);
        return new NDFA(states, alphabet, transitions, start, terminates);
    }

    private static IEnumerable<Tuple<string, string, string>> ParseTransitionTable(string source)
    {
        var matches = regexToReadTransitionTable.Matches(source);
        foreach (Match match in matches)
        {
            var line = match.Value;
            var key = line.Split("=")[0].Trim();
            var state = key.Split(".")[0];
            var symbol = key.Split(".")[1];
            var value = line.Split("=")[1].Trim();
            yield return Tuple.Create(state, symbol, value);
        }
    }
    
    private static HashSet<string> ParseTerminateStates(string source)
    {
        if (source.Length == 0)
            throw new IncorrectInputException("Поле заключительных состояний заполнено некорректно");
        var matches = regexToReadTerminateStates.Matches(source);
        return matches.Select(m => m.Value).ToHashSet();
    }
    
    private static string ParseStartState(string source)
    {
        if (source.Length == 0)
            throw new IncorrectInputException("Поле начальных состояний заполнено некорректно");
        return source.Trim();
    }
}