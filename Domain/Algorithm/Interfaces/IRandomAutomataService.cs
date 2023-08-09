using Domain.Automatas;

namespace Domain.Algorithm
{
    public interface IRandomAutomataService : IService
    {
        Automata GetRandomDFA(int statesNumber, int alphabetPower);

        Automata GetRandomNDFA(int statesNumber, int alphabetPower);
    }
}
