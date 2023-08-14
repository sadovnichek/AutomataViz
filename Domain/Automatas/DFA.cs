namespace Domain.Automatas;

public class DFA : Automata
{
    public DFA(HashSet<string> states,
        HashSet<string> alphabet,
        string startState,
        HashSet<string> terminateStates)
    {
        Alphabet = alphabet;
        Id = Guid.NewGuid();
        Transitions = new HashSet<Transition>();
        States = states;
        StartState = startState;
        TerminateStates = terminateStates;
    }

    public DFA(HashSet<string> states,
        HashSet<string> alphabet,
        HashSet<Transition> transitions,
        string startState,
        HashSet<string> terminateStates) : this(states, alphabet, startState, terminateStates)
    {
        Transitions = transitions;
    }

    public string this[string state, string symbol] => 
        Transitions.Where(x => x.State == state && x.Symbol == symbol)
                .Select(x => x.Value)
                .First();

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