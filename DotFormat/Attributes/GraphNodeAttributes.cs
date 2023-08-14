namespace DotFormat
{
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
}
