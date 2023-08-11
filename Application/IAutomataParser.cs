using Domain.Automatas;

namespace Application
{
    public interface IAutomataParser
    {
        Automata GetAutomata(string startState, string terminateStates, string transitionTable);
    }
}
