using Infrastructure;

namespace Domain.Automatas
{
    public class AutomataBuilder : IAutomataBuilder
    {
        private readonly HashSet<Transition> transitions;
        private readonly Set alphabet;
        private readonly Set states;
        private string startState;
        private readonly Set terminateStates;

        public AutomataBuilder()
        {
            states = new Set();
            terminateStates = new Set();
            alphabet = new Set();
            transitions = new HashSet<Transition>();
            startState = string.Empty;
        }

        public IAutomataBuilder AddTransition(string state, string symbol, string value)
        {
            transitions.Add(new Transition(state, symbol, value));
            alphabet.Add(symbol);
            states.Add(state);
            states.Add(value);
            return this;
        }

        public IAutomataBuilder AddTransitions(IEnumerable<Transition> transitions)
        {
            transitions.ToList()
                .ForEach(t => AddTransition(t.State, t.Symbol, t.Value));
            return this;
        }

        public IAutomataBuilder SetStartState(string startState)
        {
            this.startState = startState;
            return this;
        }

        public IAutomataBuilder SetTerminateState(string terminateState)
        {
            terminateStates.Add(terminateState);
            return this;
        }

        public IAutomataBuilder SetTerminateStates(params string[] source)
        {
            source.ToList().ForEach(s => terminateStates.Add(s));
            return this;
        }

        public IAutomataBuilder SetTerminateStates(IEnumerable<string> source)
        {
            source.ToList().ForEach(s => terminateStates.Add(s));
            return this;
        }

        public DFA BuildDFA()
        {
            return new DFA(states, alphabet, transitions, startState, terminateStates);
        }

        public NDFA BuildNDFA()
        {
            return new NDFA(states, alphabet, transitions, startState, terminateStates);
        }

        public LambdaNDFA BuildLambdaNDFA()
        {
            return new LambdaNDFA(states, alphabet, transitions, startState, terminateStates);
        }

        public Automata Build()
        {
            if (IsDfa())
               return BuildDFA();
            return alphabet.Contains(LambdaNDFA.Lambda) 
                ? BuildLambdaNDFA()
                : BuildNDFA();
        }

        private bool IsDfa()
        {
            foreach (var state in states)
            {
                foreach (var symbol in alphabet)
                {
                    if (transitions.Count(t => t.State == state && t.Symbol == symbol) != 1)
                        return false;
                }
            }
            return true;
        }
    }
}