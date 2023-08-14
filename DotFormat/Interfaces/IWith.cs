using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotFormat
{
    public interface IWith<out T> : IGraph
    {
        IGraph With(Action<T> action);
    }
}
