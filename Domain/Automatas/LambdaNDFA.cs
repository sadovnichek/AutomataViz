using Infrastructure;

namespace Domain.Automatas
{ 
    public class LambdaNDFA : NDFA
    {
        public static string Lambda => "λ";

        public LambdaNDFA(HashSet<string> states, 
            HashSet<string> alphabet, 
            string startState, 
            HashSet<string> terminateStates) : base(states, alphabet, startState, terminateStates)
        {
        }

        public LambdaNDFA(HashSet<string> states, 
            HashSet<string> alphabet, 
            HashSet<Transition> transitions, 
            string startState, 
            HashSet<string> terminateStates) : base(states, alphabet, transitions, startState, terminateStates)
        {
        }

        public Set GetLambdaClosure(string startState)
        {
            var queue = new Queue<string>();
            queue.Enqueue(startState);
            var used = new Set { startState };
            while (queue.Count != 0)
            {
                var currentState = queue.Dequeue();
                var nextStates = this[currentState, Lambda];
                foreach (var state in nextStates)
                {
                    if (!used.Contains(state))
                    {
                        queue.Enqueue(state);
                        used.Add(state);
                    }
                }
            }
            return used;
        }
    }
}
