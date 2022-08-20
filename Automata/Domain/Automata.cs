using Automata.DotFormat; // ???
using Automata.Infrastructure;

namespace Automata.Domain;

public class Automata<TState>
{
    public Table<string, TState, TState> Table { get; }
    public TState StartState { get; }
    public HashSet<TState> TerminateStates { get; }
    public HashSet<TState> States { get; }
    public HashSet<string> Alphabet { get; }
    public Guid Id = Guid.NewGuid();

    public Automata(Table<string, TState, TState> table,
        TState startState,
        HashSet<TState> terminateStates,
        HashSet<TState> states,
        HashSet<string> alphabet)
    {
        Table = table;
        StartState = startState;
        TerminateStates = terminateStates;
        States = states;
        Alphabet = alphabet;
    }

    public string ConvertToTexFormat()
    {
        var result = "\n\\begin{tabular}{ c | " + string.Join(" ", Enumerable.Repeat("c", States.Count)) + " }\n & ";
        foreach (var column in Table.Columns)
        {
            result += column.SetToString() + (Table.Columns.Last().Equals(column) ? "" : " & ");
        }

        result += " \\\\ \n\\hline\n";
        foreach (var row in Table.Rows)
        {
            result += row.SetToString() + " & ";
            foreach (var column in Table.Columns)
            {
                result += Table[column, row].SetToString() + (Table.Columns.Last().Equals(column) ? "" : " & ");
            }

            result += " \\\\\n";
        }

        result += "\\end{tabular}\n\\\\\n";
        result += $"вход: {StartState.SetToString()}, выходы: {TerminateStates.SetToString()}";
        return result;
    }

    public string ConvertToDotFormat()
    {
        var dot = DotGraphBuilder.DirectedGraph("automata");
        var edges = new Dictionary<Tuple<TState, TState>, string>();
        dot.AddNode($"START{StartState.SetToString()}").With(n => n.Color("white").FontColor("white"));
        Table.Columns.Where(state => !TerminateStates.Contains(state))
            .ToList()
            .ForEach(r => dot.AddNode(r.SetToString()).With(n => n.Shape(NodeShape.Ellipse)));
        TerminateStates.ToList()
            .ForEach(r => dot.AddNode(r.SetToString()).With(n => n.Shape(NodeShape.DoubleCircle)));
        dot.AddEdge($"START{StartState.SetToString()}", StartState.SetToString());
        foreach (var (letter, state) in Table)
        {
            var value = Table[state, letter];
            if (edges.ContainsKey(Tuple.Create(state, value)))
            {
                var newLabel = edges[Tuple.Create(state, value)] + $", {letter}";
                dot.RemoveEdge(state.SetToString(), value.SetToString());
                dot.AddEdge(state.SetToString(), value.SetToString()).With(e => e.Label(newLabel));
                edges[Tuple.Create(state, value)] = newLabel;
            }
            else
            {
                dot.AddEdge(state.SetToString(), value.SetToString()).With(e => e.Label(letter.SetToString()));
                edges[Tuple.Create(state, value)] = letter;
            }
        }

        return dot.Build();
    }

    public Automata<TState> ExceptStates(HashSet<TState> exceptedStates)
    {
        var terminates = TerminateStates.Where(s => !exceptedStates.Contains(s)).ToHashSet();
        var states = States.Where(s => !exceptedStates.Contains(s)).ToHashSet();
        var table = new Table<string, TState, TState>();
        foreach (var key in Table)
        {
            var state = key.Item2;
            var letter = key.Item1;
            if (!exceptedStates.Contains(state))
            {
                table[state, letter] = Table[state, letter];
            }
        }

        return new Automata<TState>(table, StartState, terminates, states, Alphabet);
    }

    public HashSet<TState> GetUnreachableStates()
    {
        var start = StartState;
        var used = new HashSet<TState>();
        var queue = new Queue<TState>();
        queue.Enqueue(start);
        while (queue.Count != 0)
        {
            var currentState = queue.Dequeue();
            if (used.Contains(currentState))
                continue;
            Alphabet.ToList()
                .ForEach(letter =>
                {
                    var state = Table[currentState, letter];
                    if (!queue.Contains(state))
                        queue.Enqueue(state);
                });
            used.Add(currentState);
        }

        return States.Where(s => !used.Contains(s)).ToHashSet();
    }

    public int CountCompoundSets()
    {
        if (States is HashSet<string>)
            return 0;
        if (States is HashSet<HashSet<string>> states)
            return states.Count(set => set.Count > 1);
        throw new Exception("Unexpected type of States");
    }

    public static Automata<string> GetRandom(HashSet<string> states, HashSet<string> alphabet)
    {
        var random = new Random();
        var start = states.ToList()[random.Next(1, states.Count)];
        var terminates = states.GetRandomSubset(random.Next(2, states.Count / 2 + 1));
        var table = new Table<string, string, string>();
        foreach (var state in states)
        {
            foreach (var letter in alphabet)
            {
                var randomIndex = random.Next(0, states.Count);
                table[state, letter] = states.ToList()[randomIndex];
            }
        }
        return new Automata<string>(table, start, terminates, states, alphabet);
    }

    public static Automata<string> GetRandom(int statesNumber, int alphabetNumber)
    {
        var states = Enumerable.Range(1, statesNumber)
            .Select(x => x.ToString())
            .ToHashSet();
        var alphabet = Enumerable.Range(0, alphabetNumber)
            .Select(x => ((char)(97 + x)).ToString())
            .ToHashSet();
        return GetRandom(states, alphabet);
    }
}