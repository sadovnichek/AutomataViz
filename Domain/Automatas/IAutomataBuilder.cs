namespace Domain.Automatas
{
    public interface IAutomataBuilder
    {
        IAutomataBuilder AddTransition(string state, string symbol, string value);

        IAutomataBuilder AddTransitions(IEnumerable<Transition> transitions);

        IAutomataBuilder SetStartState(string startState);

        IAutomataBuilder SetTerminateState(string terminateState);

        IAutomataBuilder SetTerminateStates(params string[] terminateStates);

        IAutomataBuilder SetTerminateStates(IEnumerable<string> terminateStates);

        DFA BuildDFA();

        NDFA BuildNDFA();

        LambdaNDFA BuildLambdaNDFA();

        Automata Build();
    }
}
