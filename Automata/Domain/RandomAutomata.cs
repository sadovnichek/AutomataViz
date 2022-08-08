using Automata.Infrastructure;

namespace Automata.Domain;

public class RandomAutomata
{
    private readonly HashSet<string> _states;
    private readonly HashSet<string> _alphabet;
    private readonly Random _random = new();
    private readonly int _size;

    public RandomAutomata(HashSet<string> states, HashSet<string> alphabet)
    {
        _states = states;
        _alphabet = alphabet;
        _size = _states.Count;
    }

    private HashSet<string> GetRandomSet(int count)
    {
        var result = new HashSet<string>();
        for (int i = 0; i < count; i++)
        {
            result.Add(_states.ToList()[_random.Next() % _size]);
        }
        return result;
    }

    public Automata<string> Get()
    {
        var start = _states.ToList()[_random.Next(1, _states.Count)];
        var terminates = GetRandomSet(_random.Next(2, _size / 2 + 1));
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