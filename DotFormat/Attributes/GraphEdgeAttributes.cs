using System.Globalization;

namespace DotFormat
{
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
}
