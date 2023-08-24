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
            var lambdaClosure = lambda.States
                .ToDictionary(s => s, s => lambda.GetLambdaClosure(s));
            var startState = lambdaClosure[lambda.StartState];
            var queue = new Queue<HashSet<string>>();
            var used = new HashSet<string>();
            queue.Enqueue(startState);
            var builder = Automata.Builder.SetStartState(startState.SetToString());
            while (queue.Count != 0)
            {
                var currentState = queue.Dequeue();
                foreach (var symbol in lambda.Alphabet.Except(LambdaNDFA.Lambda))
                {
                    var nextState = currentState.SelectMany(x => lambda[x, symbol])
                        .SelectMany(x => lambdaClosure[x]).ToHashSet();
                    builder.AddTransition(currentState.SetToString(), symbol, nextState.SetToString());
                    if (!used.Contains(nextState.SetToString()))
                        queue.Enqueue(nextState);
                }
                if (currentState.Intersect(lambda.TerminateStates).Any())
                    builder.SetTerminateState(currentState.SetToString());
                used.Add(currentState.SetToString());
            }
            return Rename(builder.BuildDFA());
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
            var transitions = automata.Transitions
                .Select(t => new Transition(naming[t.State], t.Symbol, naming[t.Value]))
                .ToHashSet();
            var terminates = automata.TerminateStates
                .Select(s => naming[s])
                .ToHashSet();
            var startState = naming[automata.StartState];
            return Automata.Builder
                .SetStartState(startState)
                .SetTerminateStates(terminates)
                .AddTransitions(transitions)
                .BuildDFA();
        }
    }
}