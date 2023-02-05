using AutomataCore.Algorithm;
using AutomataCore.Automata;
using NUnit.Framework;

namespace AutomataTest;

[TestFixture]
public class Tests
{
    private DFA dfa;
    private NDFA ndfa;

    private void ConfigureDFA()
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
    
    [SetUp]
    public void SetUp()
    {
        ConfigureDFA();
    }
    
    [Test]
    public void AcceptWord_DFA()
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
    
    /*[Test]
    public void AcceptWord_NDFA()
    {
        Assert.AreEqual(true, ndfa.IsAcceptWord("a"));
        Assert.AreEqual(false, ndfa.IsAcceptWord("aaa"));
        Assert.AreEqual(true, ndfa.IsAcceptWord("aba"));
        Assert.AreEqual(false, ndfa.IsAcceptWord("ab"));
    }*/
}