using Domain.Automatas;

namespace Domain.Services
{
    public interface IRandomAutomataService : IService
    {
        Automata GetRandom(int statesNumber, int alphabetPower, bool isDfa);
    }
}