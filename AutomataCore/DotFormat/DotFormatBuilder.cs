using System.Globalization;

namespace Automata.DotFormat;

public static class NodeShape
{
    public static string Box => "box";

    public static string Ellipse => "ellipse";

    public static string DoubleCircle => "doublecircle";
}

public interface IGraph
{
    string Build();

    IWith<GraphNodeAttributes> AddNode(string name);

    IWith<GraphEdgeAttributes> AddEdge(string from, string to);
    
    IWith<GraphEdgeAttributes> RemoveEdge(string from, string to);
}

public interface IWith<out T> : IGraph
{
    IGraph With(Action<T> action);
}

public class AttributesBuilder<T>
    where T : class
{
    protected Dictionary<string, string> Attributes = new Dictionary<string, string>();

    public T Color(string color)
    {
        Attributes.Add("color", color);
        return this as T;
    }

    public T FontSize(int fontSize)
    {
        Attributes.Add("fontsize", fontSize.ToString());
        return this as T;
    }

    public T FontColor(string fontSize)
    {
        Attributes.Add("fontcolor", fontSize.ToString());
        return this as T;
    }

    
    public T Label(string text)
    {
        Attributes.Add("label", $" {text}  ");
        return this as T;
    }
}

public class GraphNodeAttributes : AttributesBuilder<GraphNodeAttributes>
{
    public GraphNodeAttributes(GraphNode node)
    {
        Attributes = node.Attributes;
    }

    public GraphNodeAttributes Shape(string shape)
    {
        Attributes.Add("shape", shape);
        return this;
    }
}

public class GraphEdgeAttributes : AttributesBuilder<GraphEdgeAttributes>
{
    public GraphEdgeAttributes(GraphEdge edge)
    {
        Attributes = edge.Attributes;
    }

    public GraphEdgeAttributes Weight(double weight)
    {
        Attributes.Add("weight", weight.ToString(CultureInfo.InvariantCulture));
        return this;
    }
}

public class DotGraphBuilder : IWith<GraphNodeAttributes>, IWith<GraphEdgeAttributes>
{
    private static Graph _graph;

    private DotGraphBuilder(Graph graph)
    {
        _graph = graph;
    }

    public static IGraph DirectedGraph(string graphName)
    {
        return new DotGraphBuilder(new Graph(graphName, true, false));
    }

    public static IGraph UndirectedGraph(string graphName)
    {
        return new DotGraphBuilder(new Graph(graphName, false, false));
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