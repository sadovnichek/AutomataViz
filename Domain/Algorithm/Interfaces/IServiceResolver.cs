using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Algorithm
{
    public interface IServiceResolver
    {
        T GetService<T>();

        IAlgorithm GetAlgorithmByName(string name);

        IEnumerable<IAlgorithm> GetAllAlgorithms();
    }
}
