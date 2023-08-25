using System.Text.RegularExpressions;
using Domain.Automatas;
using Infrastructure;

namespace Application;

public class AutomataParser : IAutomataParser
{
    private static readonly Regex regexToReadTerminateStates = new(@"(\w+|{(.*?)})");
    private static readonly Regex regexToReadTransitionTable = new(@"(\w+|{(.*?)}|∅).\w+\s*=\s*(\w+|{(.*?)}|∅)");
    
    public Automata GetAutomata(string startState, string terminateStates, string transitionTable)
    {
        var terminates = ParseTerminateStates(terminateStates);
        var start = ParseStartState(startState);
        var transitions = ParseTransitionTable(transitionTable).ToList();
        var automata = AutomataBuilder.CreateAutomata()
            .SetStartState(start)
            .SetTerminateStates(terminates)
            .AddTransitions(transitions)
            .Build();

        if (transitions.Count == 0)
            throw new IncorrectInputException("Таблица переходов заполнена некорректно");
        if (!automata.States.Contains(start))
            throw new IncorrectInputException("Начальное состояние не указано в таблице переходов");
        if (!automata.TerminateStates.IsSubsetOf(automata.States))
        {
            var unusedStates = automata.TerminateStates
                .Where(x => !automata.States.Contains(x))
                .ToHashSet();
            throw new IncorrectInputException($"Состояния {{ {unusedStates.SetToString()} }} не указаны в таблице переходов");
        }

        return automata;
    }

    private IEnumerable<Transition> ParseTransitionTable(string source)
    {
        var matches = regexToReadTransitionTable.Matches(source);
        foreach (Match match in matches)
        {
            var line = match.Value;
            var key = line.Split("=")[0].Trim();
            var state = key.Split(".")[0];
            var symbol = key.Split(".")[1];
            var value = line.Split("=")[1].Trim();
            yield return new Transition(state, symbol, value);
        }
    }
    
    private HashSet<string> ParseTerminateStates(string source)
    {
        if (source.Length == 0)
            throw new IncorrectInputException("Поле заключительных состояний заполнено некорректно");
        var matches = regexToReadTerminateStates.Matches(source);
        return matches.Select(m => m.Value).ToHashSet();
    }
    
    private string ParseStartState(string source)
    {
        if (source.Length == 0)
            throw new IncorrectInputException("Поле начальных состояний заполнено некорректно");
        return source.Trim();
    }
}