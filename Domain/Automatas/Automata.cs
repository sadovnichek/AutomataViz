using System.Text;

namespace Domain.Automatas;

public abstract class Automata
{
    public HashSet<Tuple<string, string, string>> Transitions { get; protected init; }

    public Guid Id = Guid.NewGuid();

    public HashSet<string> Alphabet { get; protected init; }

    public HashSet<string> States { get; protected init;}

    public string StartState { get; protected init;}

    public HashSet<string> TerminateStates { get; protected init;}

    public void AddTransition(string state, string symbol, string value)
    {
        Transitions.Add(Tuple.Create(state, symbol, value));
    }

    public string GetTransitionTableFormatted()
    {
        var output = new StringBuilder();
        foreach (var groups in Transitions.GroupBy(x => x.Item1))
        {
            foreach (var (item1, item2, item3) in groups)
            {
                output.Append($"{item1}.{item2} = {item3}");
                output.Append('\t');
            }
            output.Append('\n');
        }
        return output.ToString();
    }

    public static bool IsDfa(
        HashSet<Tuple<string, string, string>> transitions,
        HashSet<string> alphabet,
        HashSet<string> states)
    {
        foreach (var state in states)
        {
            foreach (var symbol in alphabet)
            {
                if (transitions.Count(t => t.Item1 == state && t.Item2 == symbol) != 1)
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