using Domain.Automatas;

namespace Domain.Algorithm
{
    public interface IRandomAutomataService : IService
    {
        Automata GetRandom(int statesNumber, int alphabetPower, bool isDfa);
    }
}