namespace Domain.Automatas;

public class DFA : Automata
{
    public DFA(HashSet<string> states,
        HashSet<string> alphabet,
        HashSet<Transition> transitions,
        string startState,
        HashSet<string> terminateStates)
    {
        Alphabet = alphabet;
        Id = Guid.NewGuid();
        Transitions = new HashSet<Transition>();
        States = states;
        StartState = startState;
        TerminateStates = terminateStates;
        Transitions = transitions;
    }

    public string this[string state, string symbol] => 
        Transitions.Where(x => x.State == state && x.Symbol == symbol)
                .Select(x => x.Value)
                .First();

    public override DFA ExceptStates(HashSet<string> exceptedStates)
    {
        var terminates = TerminateStates.Where(s => !exceptedStates.Contains(s)).ToArray();
        var newAutomata = Builder.SetStartState(StartState).SetTerminateStates(terminates);
        foreach (var state in States.Where(s => !exceptedStates.Contains(s)))
        {
            foreach (var symbol in Alphabet)
            {
                var value = this[state, symbol];
                if (!exceptedStates.Contains(value))
                    newAutomata.AddTransition(state, symbol, value);
            }
        }
        return newAutomata.BuildDFA();
    }

    public override HashSet<string> GetUnreachableStates()
    {
        var used = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(StartState);

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
        foreach (var symbol in word)
        {
            currentState = this[currentState, symbol.ToString()];
        }
        return TerminateStates.Contains(currentState);
    }
}