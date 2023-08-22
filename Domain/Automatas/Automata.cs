using System.Text;

namespace Domain.Automatas;

public abstract class Automata
{
    public HashSet<Transition> Transitions { get; protected init; }

    public Guid Id { get; protected init; }

    public HashSet<string> Alphabet { get; protected set; }

    public HashSet<string> States { get; protected set; }

    public string StartState { get; protected set; }

    public HashSet<string> TerminateStates { get; protected set; }

    public Automata AddTransition(string state, string symbol, string value)
    {
        Transitions.Add(new Transition(state, symbol, value));
        return this;
    }

    public string GetTransitionTableFormatted()
    {
        var output = new StringBuilder();
        foreach (var groups in Transitions.GroupBy(x => x.State))
        {
            foreach (var transition in groups)
            {
                output.Append($"{transition.State}.{transition.Symbol} = {transition.Value}");
                output.Append('\t');
            }
            output.Append('\n');
        }
        return output.ToString();
    }

    public Automata ExceptUnreachableStates()
    {
        return ExceptStates(GetUnreachableStates());
    }

    public static bool IsDfa(
        HashSet<Transition> transitions,
        HashSet<string> alphabet,
        HashSet<string> states)
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

    public override bool Equals(object? obj)
    {
        if (obj is not Automata other)
            return false;
        return StartState.Equals(other.StartState) &&
            Alphabet.SetEquals(other.Alphabet) &&
            Transitions.SetEquals(other.Transitions) &&
            States.SetEquals(other.States) &&
            TerminateStates.SetEquals(other.TerminateStates);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Transitions, Alphabet, States, StartState, TerminateStates);
    }

    public abstract bool IsRecognizeWord(string word); 

    public abstract HashSet<string> GetUnreachableStates();

    public abstract Automata ExceptStates(HashSet<string> exceptedStates);
}