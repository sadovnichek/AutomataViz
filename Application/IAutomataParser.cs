using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Automatas;

namespace Application
{
    public interface IAutomataParser
    {
        Automata GetAutomata(string startState, string terminateStates, string transitionTable);
    }
}
