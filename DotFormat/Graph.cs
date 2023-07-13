namespace DotFormat;

public class Graph
{
    private readonly List<GraphEdge> edges = new();
    private readonly Dictionary<string, GraphNode> nodes = new();

    public Graph(string graphName, bool directed, bool strict)
    {
        GraphName = graphName;
        Directed = directed;
        Strict = strict;
    }

    public string GraphName { get; }
    public bool Directed { get; }
    public bool Strict { get; }

    public IEnumerable<GraphEdge> Edges => edges;
    public IEnumerable<GraphNode> Nodes => nodes.Values;

    public GraphNode AddNode(string name)
    {
        if (!nodes.TryGetValue(name, out var result))
            nodes.Add(name, result = new GraphNode(name));
        return result;
    }

    public GraphEdge AddEdge(string sourceNode, string destinationNode)
    {
        var result = new GraphEdge(sourceNode, destinationNode, Directed);
        edges.Add(result);
        return result;
    }
    
    public void RemoveEdge(string sourceNode, string destinationNode)
    {
        var result = edges.First(e => e.SourceNode == sourceNode && e.DestinationNode == destinationNode);
        edges.Remove(result);
    }
}