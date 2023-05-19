using Domain.Automatas;
using Infrastructure.DotFormat;

namespace Application
{
    public class ConvertAutomataToDotFormat
    {
        public static string Convert(Automata automata)
        {
            var dot = DotGraphBuilder.DirectedGraph("automata");
            var edges = new Dictionary<Tuple<string, string>, string>(); // <from, to>, label
            dot.AddNode($"START{automata.StartState}").With(n => n.Color("white").FontColor("white"));
            automata.States.Where(state => !automata.TerminateStates.Contains(state))
                .ToList()
                .ForEach(r => dot.AddNode(r).With(n => n.Shape(NodeShape.Ellipse)));
            automata.TerminateStates.ToList()
                .ForEach(r => dot.AddNode(r).With(n => n.Shape(NodeShape.DoubleCircle)));
            dot.AddEdge($"START{automata.StartState}", automata.StartState);
                foreach (var (from, symbol, to) in automata.Transitions)
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
    }
}
