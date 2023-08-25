namespace DotFormat;

public class DotFormatBuilder : IWith<GraphNodeAttributes>, IWith<GraphEdgeAttributes>
{
    private static Graph _graph;

    public IGraph GetDirectedGraph(string graphName)
    {
        _graph = new Graph(graphName, true, false);
        return this;
    }

    public IGraph GetUndirectedGraph(string graphName)
    {
        _graph = new Graph(graphName, false, false);
        return this;
    }

    public string Build() => _graph.ToDotFormat();

    public IWith<GraphNodeAttributes> AddNode(string name)
    {
        _graph.AddNode(name);
        return this;
    }

    public IWith<GraphEdgeAttributes> AddEdge(string from, string to)
    {
        _graph.AddEdge(from, to);
        return this;
    }

    public IWith<GraphEdgeAttributes> RemoveEdge(string from, string to)
    {
        _graph.RemoveEdge(from, to);
        return this;
    }
    
    public IGraph With(Action<GraphNodeAttributes> action)
    {
        action(new GraphNodeAttributes(_graph.Nodes.Last()));
        return this;
    }

    public IGraph With(Action<GraphEdgeAttributes> action)
    {
        action(new GraphEdgeAttributes(_graph.Edges.Last()));
        return this;
    }
}