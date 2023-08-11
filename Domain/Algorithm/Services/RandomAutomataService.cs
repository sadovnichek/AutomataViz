using Domain.Automatas;
using Infrastructure;

namespace Domain.Algorithm.Services
{
    public class RandomAutomataService : IRandomAutomataService
    {
        private readonly Random random;

        public RandomAutomataService()
        {
            random = new Random();
        }

        public Automata GetRandomDFA(int statesNumber, int alphabetPower)
        {
            var states = Enumerable.Range(1, statesNumber)
                .Select(x => x.ToString())
                .ToHashSet();
            var alphabet = Enumerable.Range(0, alphabetPower)
                .Select(x => ((char)(97 + x)).ToString())
                .ToHashSet();
            return GetRandomDFA(states, alphabet);
        }

        private DFA GetRandomDFA(HashSet<string> states, HashSet<string> alphabet)
        {
            var startState = states.ToList()[random.Next(1, states.Count)];
            var terminateStates = states.GetRandomSubset(random.Next(2, states.Count / 2 + 1));
            var randomAutomata = new DFA(states, alphabet, startState, terminateStates);
            foreach (var state in states)
            {
                foreach (var symbol in alphabet)
                {
                    var randomIndex = random.Next(0, states.Count);
                    randomAutomata.AddTransition(state, symbol, states.ToList()[randomIndex]);
                }
            }
            return randomAutomata;
        }

        public Automata GetRandomNDFA(int statesNumber, int alphabetNumber)
        {
            var states = Enumerable.Range(1, statesNumber)
                .Select(x => x.ToString())
                .ToHashSet();
            var alphabet = Enumerable.Range(0, alphabetNumber)
                .Select(x => ((char)(97 + x)).ToString())
                .ToHashSet();
            return GetRandomNDFA(states, alphabet);
        }

        private static NDFA GetRandomNDFA(HashSet<string> states, HashSet<string> alphabet)
        {
            var random = new Random();
            var start = states.ToList()[random.Next(0, states.Count)];
            var terminates = states.GetRandomSubset(random.Next(2, states.Count / 2 + 1));
            var randomAutomata = new NDFA(states, alphabet, start, terminates);
            foreach (var state in states)
            {
                foreach (var symbol in alphabet)
                {
                    for (int i = 0; i < random.Next(0, 3); i++)
                    {
                        var randomIndex = random.Next(0, states.Count);
                        randomAutomata.AddTransition(state, symbol, states.ToList()[randomIndex]);
                    }
                }
            }
            return randomAutomata;
        }
    }
}
