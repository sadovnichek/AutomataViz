using System.Text;
using Infrastructure;

namespace Domain.Automatas;

public class DFA : Automata
{
    public DFA(HashSet<string> states,
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

    public DFA(HashSet<string> states,
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

    public string this[string state, string symbol] => 
        Transitions.Where(x => x.Item1 == state && x.Item2 == symbol)
                .Select(x => x.Item3)
                .First();
    
    public override string ConvertToTexFormat()
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
                sb.Append(this[state, symbol].StringToSet().SetToString(true) + (States.Last().Equals(state) ? "" : " & "));
            }
            sb.Append(" \\\\\n");
        }
        sb.Append("\\end{tabular}\n\\\\\n");
        sb.Append($"вход: {StartState.StringToSet().SetToString(true)}, выходы: {TerminateStates.SetToString(true)}");
        return sb.ToString();
    }
    
    public override DFA ExceptStates(HashSet<string> exceptedStates)
    {
        var newTerminates = TerminateStates.Where(s => !exceptedStates.Contains(s)).ToHashSet();
        var newStates = States.Where(s => !exceptedStates.Contains(s)).ToHashSet();
        var newAutomata = new DFA(newStates, Alphabet, StartState, newTerminates);

        foreach (var state in newStates)
        {
            foreach (var symbol in Alphabet)
            {
                var value = this[state, symbol];
                if (!exceptedStates.Contains(value))
                {
                    newAutomata.AddTransition(state, symbol, value);
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
                    var state = this[currentState, letter];
                    if (!queue.Contains(state))
                        queue.Enqueue(state);
                });
            used.Add(currentState);
        }

        return States.Where(s => !used.Contains(s)).ToHashSet();
    }

    public override bool IsRecognizeWord(string word)
    {
        var currentState = StartState;
        foreach (var w in word)
        {
            currentState = this[currentState, w.ToString()];
        }
        return TerminateStates.Contains(currentState);
    }
}