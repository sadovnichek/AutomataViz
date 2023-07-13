using System.Text.RegularExpressions;

namespace DotFormat;

public class DotFormatWriter
{
    private readonly TextWriter _writer;

    public DotFormatWriter(TextWriter writer)
    {
        _writer = writer;
    }

    public void Write(Graph graph)
    {
        _writer.Write(
            "{0}graph {1} {{\n",
            graph.Directed ? "di" : "",
            EscapeId(graph.GraphName)
        );
        _writer.Write("    concentrate=true\n");
        var isFirst = true;
        foreach (dynamic item in graph.Nodes.Cast<object>().Concat(graph.Edges))
        {
            if (!isFirst) _writer.Write(";\n");
            isFirst = false;
            Write(item);
        }

        if (!isFirst) _writer.Write("\n");
        _writer.Write("}");
    }

    public void Write(GraphNode node)
    {
        _writer.Write("    " + EscapeId(node.Name));
        WriteAttributes(node.Attributes);
    }

    public void Write(GraphEdge edge)
    {
        var edgeSign = edge.Directed ? "->" : "--";
        _writer.Write("    {0} {1} {2}", EscapeId(edge.SourceNode), edgeSign, EscapeId(edge.DestinationNode));
        WriteAttributes(edge.Attributes);
    }

    public void WriteAttributes(IReadOnlyDictionary<string, string> attributes)
    {
        if (attributes.Count == 0) return;
        var attributesStr = attributes.OrderBy(a => a.Key).Select(a => EscapeId(a.Key) + "=" + EscapeId(a.Value));
        var attrs = string.Join("; ", attributesStr);
        _writer.Write(" [{0}]", attrs);
    }

    public static string EscapeId(string id)
    {
        if (Regex.IsMatch(id, @"^[a-zA-Z_][a-zA-Z_0-9]*$") ||
            Regex.IsMatch(id, @"^[-]?(\.[0-9]+|[0-9]+(\.[0-9]*)?)$"))
            return id;
        return $"\"{id.Replace("\"", "\\\"")}\"";
    }
}

public static class DotFormatExtensions
{
    public static string ToDotFormat(this Graph graph)
    {
        using var s = new StringWriter();
        new DotFormatWriter(s).Write(graph);
        s.Flush();
        return s.ToString();
    }
}