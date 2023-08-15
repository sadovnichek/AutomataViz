using Domain.Automatas;
using Infrastructure;

namespace Domain.Services
{
    public class RandomAutomataService : IRandomAutomataService
    {
        private readonly Random random;

        public RandomAutomataService()
        {
            random = new Random();
        }

        public Automata GetRandom(int statesNumber, int alphabetPower, bool isDfa)
        {
            var states = Enumerable.Range(1, statesNumber)
                .Select(x => x.ToString())
                .ToHashSet();
            var alphabet = Enumerable.Range(0, alphabetPower)
                .Select(x => ((char)(97 + x)).ToString())
                .ToHashSet();
            return GetRandom(states, alphabet, isDfa);
        }

        private Automata GetRandom(HashSet<string> states, HashSet<string> alphabet, bool isDfa)
        {
            while (true)
            {
                var start = states.ToList()[random.Next(0, states.Count)];
                var terminates = states.GetRandomSubset(random.Next(2, states.Count / 2 + 1));
                var randomAutomata = new NDFA(states, alphabet, start, terminates);
                foreach (var state in states)
                {
                    foreach (var symbol in alphabet)
                    {
                        var iteration = 0;
                        do
                        {
                            var randomIndex = random.Next(0, states.Count);
                            randomAutomata.AddTransition(state, symbol, states.ToList()[randomIndex]);
                            iteration++;
                        } while (iteration < random.Next(1, 3) && !isDfa);
                    }
                }
                var unreachableStates = randomAutomata.GetUnreachableStates();
                if (!randomAutomata.TerminateStates.Intersect(unreachableStates).Any())
                    return randomAutomata;
            }
        }
    }
}