using Automata.Infrastructure;

namespace Automata.Domain;

public class RandomAutomata
{
    private readonly HashSet<string> _states;
    private readonly HashSet<string> _alphabet;
    private readonly Random _random = new();

    public RandomAutomata(HashSet<string> states, HashSet<string> alphabet)
    {
        _states = states;
        _alphabet = alphabet;
    }

    public RandomAutomata(int statesNumber, int alphabetNumber)
    {
        _states = Enumerable.Range(1, statesNumber).Select(x => x.ToString()).ToHashSet();
        _alphabet = Enumerable.Range(0, alphabetNumber).Select(x => ((char)(97 + x)).ToString()).ToHashSet();
    }
    
    private HashSet<string> GetRandomSubset(int count)
    {
        var result = new HashSet<string>();
        var values = Enumerable.Range(1, count).Select(x => x.ToString()).ToHashSet();
        for (int i = 0; i < count; i++)
        {
            result.Add(values.ToList()[_random.Next() % values.Count]);
        }
        return result;
    }

    public Automata<string> Get()
    {
        var start = _states.ToList()[_random.Next(1, _states.Count)];
        var terminates = GetRandomSubset(_random.Next(2, _states.Count / 2 + 1));
        var table = new Table<string, string, string>();
        foreach (var state in _states)
        {
            foreach (var letter in _alphabet)
            {
                var randomIndex = _random.Next(0, _states.Count);
                table[state, letter] = _states.ToList()[randomIndex];
            }
        }
        return new Automata<string>(table, start, terminates, _states, _alphabet);
    }
}