using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
