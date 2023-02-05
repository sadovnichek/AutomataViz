using AutomataCore.Algorithm;
using AutomataCore.Automata;
using Infrastructure;
using NUnit.Framework;

namespace AutomataTest;

[TestFixture]
public class Tests
{
    private DFA dfa;
    private NDFA ndfa;

    private void ConfigureDfa()
    {
        var states = new HashSet<string> {"0", "1", "2", "3", "4", "5", "6"};
        var alphabet = new HashSet<string> {"a", "b"};
        var terminates = new HashSet<string> {"4", "5", "6"};
        dfa = new DFA(states, alphabet, "0", terminates);
        dfa.AddTransition("0", "a", "5");
        dfa.AddTransition("0", "b", "2");
        dfa.AddTransition("1", "a", "6");
        dfa.AddTransition("1", "b", "2");
        dfa.AddTransition("2", "a", "0");
        dfa.AddTransition("2", "b", "4");
        dfa.AddTransition("3", "a", "3");
        dfa.AddTransition("3", "b", "5");
        dfa.AddTransition("4", "a", "6");
        dfa.AddTransition("4", "b", "2");
        dfa.AddTransition("5", "a", "3");
        dfa.AddTransition("5", "b", "0");
        dfa.AddTransition("6", "a", "3");
        dfa.AddTransition("6", "b", "1");
    }

    private void ConfigureNdfa()
    {
        var states = new HashSet<string> {"0", "1", "2"};
        var alphabet = new HashSet<string> {"a", "b"};
        var terminates = new HashSet<string> {"0"};
        ndfa = new NDFA(states, alphabet, "0", terminates);
        ndfa.AddTransition("0", "a", "1");
        ndfa.AddTransition("1", "b", "0");
        ndfa.AddTransition("1", "b", "2");
        ndfa.AddTransition("2", "a", "0");
    }
    
    [SetUp]
    public void SetUp()
    {
        ConfigureDfa();
        ConfigureNdfa();
    }
    
    [Test]
    public void AcceptWordDfa()
    {
        Assert.AreEqual(true, dfa.IsAcceptWord("a"));
        Assert.AreEqual(false, dfa.IsAcceptWord("aaa"));
        Assert.AreEqual(false, dfa.IsAcceptWord("aabaa"));
        Assert.AreEqual(true, dfa.IsAcceptWord("aab"));
    }

    [Test]
    public void DfaMinimization()
    {
        var actual = AlgorithmResolver.GetService<MinimizationAlgorithm>().Get(dfa);
        var expectedStates = new HashSet<string> {"{0, 1}", "2", "3", "4", "{5, 6}"};
        var expectedAlphabet = new HashSet<string> {"a", "b"};
        var expectedStart = "{0, 1}";
        var expectedTerminates = new HashSet<string> {"4", "{5, 6}"};
        var expectedTransitions = new HashSet<Tuple<string, string, string>>
        {
            Tuple.Create("{0, 1}", "a", "{5, 6}"),
            Tuple.Create("{0, 1}", "b", "2"),
            Tuple.Create("2", "a", "{0, 1}"),
            Tuple.Create("2", "b", "4"),
            Tuple.Create("3", "a", "3"),
            Tuple.Create("3", "b", "{5, 6}"),
            Tuple.Create("4", "a", "{5, 6}"),
            Tuple.Create("4", "b", "2"),
            Tuple.Create("{5, 6}", "a", "3"),
            Tuple.Create("{5, 6}", "b", "{0, 1}")
        };
        Assert.IsTrue(actual.States.SetEquals(expectedStates));
        Assert.IsTrue(actual.Alphabet.SetEquals(expectedAlphabet));
        Assert.AreEqual(expectedStart, actual.StartState);
        Assert.IsTrue(actual.TerminateStates.SetEquals(expectedTerminates));
        Assert.IsTrue(actual.Transitions.SetEquals(expectedTransitions));
    }
    
    [Test]
    public void AcceptWordNDFA()
    {
        Assert.AreEqual(false, ndfa.IsAcceptWord("a"));
        Assert.AreEqual(false, ndfa.IsAcceptWord("aaa"));
        Assert.AreEqual(true, ndfa.IsAcceptWord("aba"));
        Assert.AreEqual(true, ndfa.IsAcceptWord("ab"));
    }

    [Test]
    public void DeterminateNdfa()
    {
        var actual = AlgorithmResolver.GetService<DeterminizationAlgorithm>().Get(ndfa);
        var expectedStates = new HashSet<string> {"0", "1", "Ø", "{0, 2}", "{0, 1}"};
        var expectedAlphabet = new HashSet<string> {"a", "b"};
        var expectedStart = "0";
        var expectedTerminates = new HashSet<string> {"0", "{0, 1}", "{0, 2}"};
        var expectedTransitions = new HashSet<Tuple<string, string, string>>
        {
            Tuple.Create("0", "a", "1"),
            Tuple.Create("0", "b", "Ø"),
            Tuple.Create("1", "a", "Ø"),
            Tuple.Create("1", "b", "{0, 2}"),
            Tuple.Create("Ø", "a", "Ø"),
            Tuple.Create("Ø", "b", "Ø"),
            Tuple.Create("{0, 2}", "a", "{0, 1}"),
            Tuple.Create("{0, 2}", "b",  "Ø"),
            Tuple.Create("{0, 1}", "a", "1"),
            Tuple.Create("{0, 1}", "b", "{0, 2}"),
        };
        Assert.IsTrue(actual.States.SetEquals(expectedStates));
        Assert.IsTrue(actual.Alphabet.SetEquals(expectedAlphabet));
        Assert.AreEqual(expectedStart, actual.StartState);
        Assert.IsTrue(actual.TerminateStates.SetEquals(expectedTerminates));
        Assert.IsTrue(actual.Transitions.SetEquals(expectedTransitions));
    }

    [Test]
    public void IsNotDfa()
    {
        Assert.IsFalse(Automata.IsDfa(ndfa.Transitions, ndfa.Alphabet, ndfa.States));
    }
    
    [Test]
    public void IsDfa()
    {
        Assert.IsTrue(Automata.IsDfa(dfa.Transitions, dfa.Alphabet, dfa.States));
    }

    [Test]
    public void CountCompoundSetsTest()
    {
        var states = new HashSet<string> {"0", "1", "Ø", "{0, 2}", "{0, 1}"};
        var automata = new DFA(states, 
            Enumerable.Empty<string>().ToHashSet(), string.Empty, Enumerable.Empty<string>().ToHashSet());
        Assert.AreEqual(2, automata.CountCompoundSets());
    }

    [Test]
    public void SetToStringTest()
    {
        var set = new HashSet<string> { "1", "2", "3"};
        Assert.AreEqual("{1, 2, 3}", set.SetToString());
        set = new HashSet<string> { "1" };
        Assert.AreEqual("1", set.SetToString());
    }

    [Test]
    public void StringToSetTest()
    {
        var str = "{1, 2, 3}";
        Assert.AreEqual(new HashSet<string> { "1", "2", "3"}, str.StringToSet());
    }
}