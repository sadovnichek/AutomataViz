using Domain.Automatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Algorithm
{
    public interface IService
    {

    }

    public interface IVisualizationService : IService
    {
        Uri GetImageUri(Automata automata);
    }
}
