using System.ComponentModel;

namespace DotFormat
{
    public interface INodeShape
    {
        public string Box { get; }

        public string Ellipse { get; }

        public string DoubleCircle { get; }
    }

    public class NodeShape : INodeShape
    {
        public string Box => "box";

        public string Ellipse => "ellipse";

        public string DoubleCircle => "doublecircle";
    }
}
