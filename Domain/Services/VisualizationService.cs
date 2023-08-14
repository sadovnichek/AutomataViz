using Domain.Automatas;
using DotFormat;
using System.Diagnostics;

namespace Domain.Services
{
    public class VisualizationService : IVisualizationService
    {
        public Uri GetImageUri(Automata automata)
        { 
            var imagePath = GetImagePath(automata);
            var uri = new Uri(imagePath);
            return uri;
        }

        public string GetBase64Image(Automata automata)
        {
            var imagePath = GetImagePath(automata);
            var bytes = File.ReadAllBytes(imagePath);
            File.Delete(imagePath);
            return Convert.ToBase64String(bytes);
        }

        private string GetImagePath(Automata automata)
        {
            var dotFormat = ConvertAutomataToDotFormat(automata);
            var dotFileName = $"./{automata.Id}.dot";
            var imageFileName = Directory.GetCurrentDirectory() + $"/images/{automata.Id}.png";
            File.WriteAllText(dotFileName, dotFormat);
            GenerateImage(dotFileName, imageFileName);
            File.Delete(dotFileName);
            return imageFileName;
        }

        private string ConvertAutomataToDotFormat(Automata automata)
        {
            var dot = DotFormatBuilder.DirectedGraph("automata");
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