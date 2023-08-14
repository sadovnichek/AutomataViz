using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotFormat
{
    public interface IGraph
    {
        string Build();

        IWith<GraphNodeAttributes> AddNode(string name);

        IWith<GraphEdgeAttributes> AddEdge(string from, string to);

        IWith<GraphEdgeAttributes> RemoveEdge(string from, string to);
    }
}
