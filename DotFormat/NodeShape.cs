using System.ComponentModel;

namespace DotFormat
{
    public interface INodeShape
    {
        public string Box { get; }

        public string Circle { get; }

        public string DoubleCircle { get; }
    }

    public class NodeShape : INodeShape
    {
        public string Box => "box";

        public string Circle => "circle";

        public string DoubleCircle => "doublecircle";
    }
}
