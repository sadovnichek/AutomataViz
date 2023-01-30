using System.Text;
using Automata.DotFormat;
using Automata.Infrastructure;

namespace Automata;

public abstract class Automata
{
    protected HashSet<Tuple<string, string, string>> Transitions = new();
    public Guid Id = Guid.NewGuid();
    public HashSet<string> Alphabet { get; protected init; }
    public HashSet<string> States { get; protected init;}
    public string StartState { get; protected init;}
    public HashSet<string> TerminateStates { get; protected init;}

    public void AddTransition(string state, string symbol, string value)
    {
        Transitions.Add(Tuple.Create(state, symbol, value));
    }

    public string ConvertToDotFormat()
    {
        var dot = DotGraphBuilder.DirectedGraph("automata");
        var edges = new Dictionary<Tuple<string, string>, string>(); // <from, to>, label

        dot.AddNode($"START{StartState}").With(n => n.Color("white").FontColor("white"));
        States.Where(state => !TerminateStates.Contains(state))
            .ToList()
            .ForEach(r => dot.AddNode(r).With(n => n.Shape(NodeShape.Ellipse)));
        TerminateStates.ToList()
            .ForEach(r => dot.AddNode(r).With(n => n.Shape(NodeShape.DoubleCircle)));
        dot.AddEdge($"START{StartState}", StartState);

        foreach (var (from, symbol, to) in Transitions)
        {
            if (edges.ContainsKey(Tuple.Create(from, to)))
            {
                var newLabel = edges[Tuple.Create(from, to)] + $", {symbol}";
                dot.RemoveEdge(from, to);
                dot.AddEdge(from, to).With(e => e.Label(newLabel));
                edges[Tuple.Create(from, to)] = newLabel;
            }
            else
            {
                dot.AddEdge(from, to).With(e => e.Label(symbol));
                edges[Tuple.Create(from, to)] = symbol;
            }
        }

        return dot.Build();
    }
    
    public string GetTextForm()
    {
        var output = new StringBuilder();
        foreach (var groups in Transitions.GroupBy(x => x.Item1))
        {
            foreach (var (item1, item2, item3) in groups)
            {
                output.Append($"{item1}.{item2} = {item3}");
                output.Append('\t');
            }

            output.Append('\n');
        }

        return output.ToString();
    }
    
    public int CountCompoundSets()
    {
        return States.Count(s => s.StringToSet().Count > 1);
    }
    
    public abstract bool IsAcceptWord(string word);
}