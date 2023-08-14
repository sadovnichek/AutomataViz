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
        HashSet<Transition> transitions,
        string startState,
        HashSet<string> terminateStates)
    {
        Transitions = transitions;
        Alphabet = alphabet;
        States = states;
        StartState = startState;
        TerminateStates = terminateStates;
    }

    public HashSet<string> this[string state, string symbol] =>
        Transitions.Where(x => x.State == state && x.Symbol == symbol)
                .Select(x => x.Value)
                .ToHashSet();

    public override NDFA ExceptStates(HashSet<string> exceptedStates)
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

    public override bool IsRecognizeWord(string word)
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