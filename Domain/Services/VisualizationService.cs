using Domain.Automatas;
using DotFormat;
using System.Diagnostics;

namespace Domain.Services
{
    public class VisualizationService : IVisualizationService
    {
        private readonly IGraph dotFormatBuilder;
        private readonly INodeShape nodeShape;

        public VisualizationService(IGraph dotFormatBuilder, 
            INodeShape nodeShape)
        {
            this.dotFormatBuilder = dotFormatBuilder;
            this.nodeShape = nodeShape;
        }

        public void SaveAutomataImage(Automata automata, string filePath)
        {
            var dotFormat = ConvertAutomataToDotFormat(automata);
            var dotFileName = $"./{automata.Id}.dot";
            File.WriteAllText(dotFileName, dotFormat);
            GenerateImage(dotFileName, filePath);
            File.Delete(dotFileName);
        }

        private string ConvertAutomataToDotFormat(Automata automata)
        {
            var dot = dotFormatBuilder.GetDirectedGraph("automata");
            var edges = new Dictionary<Tuple<string, string>, string>(); // <from, to>, label
            dot.AddNode($"START{automata.StartState}")
                .With(n => n.Color("white").FontColor("white"));
            automata.States
                .Where(state => !automata.TerminateStates.Contains(state))
                .ToList()
                .ForEach(r => dot.AddNode(r).With(n => n.Shape(nodeShape.Ellipse)));
            automata.TerminateStates
                .ToList()
                .ForEach(r => dot.AddNode(r).With(n => n.Shape(nodeShape.DoubleCircle)));
            dot.AddEdge($"START{automata.StartState}", automata.StartState);
            foreach (var t in automata.Transitions)
            {
                if (edges.ContainsKey(Tuple.Create(t.State, t.Value)))
                {
                    var newLabel = edges[Tuple.Create(t.State, t.Value)] + $", {t.Symbol}";
                    dot.RemoveEdge(t.State, t.Value);
                    dot.AddEdge(t.State, t.Value).With(e => e.Label(newLabel));
                    edges[Tuple.Create(t.State, t.Value)] = newLabel;
                }
                else
                {
                    dot.AddEdge(t.State, t.Value).With(e => e.Label(t.Symbol));
                    edges[Tuple.Create(t.State, t.Value)] = t.Symbol;
                }
            }
            return dot.Build();
        }

        private void GenerateImage(string sourceFileName, string destinationFileName)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "./bin/dot.exe",
                    UseShellExecute = false,
                    Arguments = $"{sourceFileName} -Tpng -o {destinationFileName}"
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}