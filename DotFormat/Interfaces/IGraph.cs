namespace DotFormat
{
    public interface IGraph
    {
        string Build();

        IGraph GetDirectedGraph(string graphName);

        IGraph GetUndirectedGraph(string graphName);

        IWith<GraphNodeAttributes> AddNode(string name);

        IWith<GraphEdgeAttributes> AddEdge(string from, string to);

        IWith<GraphEdgeAttributes> RemoveEdge(string from, string to);
    }
}
