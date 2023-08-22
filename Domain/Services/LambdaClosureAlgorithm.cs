using Domain.Automatas;
using Infrastructure;

namespace Domain.Services
{
    public class LambdaClosureAlgorithm : IAlgorithmTransformer
    {
        public string Name => "Приведение λ-НКА к ДКА";

        public Automata Get(Automata automata)
        {
            if (automata is not LambdaNDFA lambda)
                throw new InvalidOperationException("Автомат должен быть λ-НКА");
            var states = new HashSet<string>();
            var transitions = new HashSet<Transition>();
            var terminates = new HashSet<string>();
            var lambdaClosureMapping = lambda.States
                .ToDictionary(s => s, s => lambda.GetLambdaClosure(s));
            var startState = lambdaClosureMapping[lambda.StartState];
            var queue = new Queue<HashSet<string>>();
            var used = new HashSet<string>();
            queue.Enqueue(startState);
            states.Add(startState.SetToString());
            while (queue.Count != 0)
            {
                var currentState = queue.Dequeue();
                foreach (var symbol in lambda.Alphabet.Except(LambdaNDFA.Lambda))
                {
                    var value = new List<string>();
                    foreach (var state in currentState)
                    {
                        value.AddRange(lambda[state, symbol]);
                    }
                    var nextState = value.SelectMany(x => lambdaClosureMapping[x])
                        .ToHashSet();
                    states.Add(nextState.SetToString());
                    transitions.Add(new Transition(currentState.SetToString(), symbol, nextState.SetToString()));
                    if (!used.Contains(nextState.SetToString()))
                        queue.Enqueue(nextState);
                }
                if (currentState.Intersect(lambda.TerminateStates).Any())
                    terminates.Add(currentState.SetToString());
                used.Add(currentState.SetToString());
            }
            var result = new DFA(states, automata.Alphabet, transitions, startState.SetToString(), terminates);
            return Rename(result);
        }

        private DFA Rename(Automata automata)
        {
            var index = 0;
            var naming = new Dictionary<string, string>();
            foreach (var state in automata.States.Except("Ø"))
            {
                naming.Add(state, index.ToString());
                index++;
            }
            naming.Add("Ø", "Ø");
            var states = automata.States.Select(s => naming[s]).ToHashSet();
            var transitions = automata.Transitions
                .Select(t => new Transition(naming[t.State], t.Symbol, naming[t.Value]))
                .ToHashSet();
            var terminates = automata.TerminateStates.Select(s => naming[s]).ToHashSet();
            var startState = naming[automata.StartState];
            return new DFA(states, automata.Alphabet, transitions, startState, terminates);
        }
    }
}