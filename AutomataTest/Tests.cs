using Domain.Algorithm;
using Domain.Automatas;
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
    
    [OneTimeSetUp]
    public void SetUp()
    {
        ConfigureDfa();
        ConfigureNdfa();
    }
    
    [Test]
    [TestCase("a", true)]
    [TestCase("aaa", false)]
    [TestCase("aabaa", false)]
    [TestCase("aab", true)]
    public void Dfa_IsRecognizeWord_Correctly(string word, bool expected)
    {
        Assert.AreEqual(expected, dfa.IsRecognizeWord(word));
    }

    [Test]
    [TestCase("a", false)]
    [TestCase("aaa", false)]
    [TestCase("aba", true)]
    [TestCase("ab", true)]
    public void Ndfa_IsRecognizeWord_Correctly(string word, bool expected)
    {
        Assert.AreEqual(expected, ndfa.IsRecognizeWord(word));
    }

    [Test]
    public void MinimizationAlgorithm_Get_CorrectDfa()
    {
        var states = new HashSet<string> {"{0, 1}", "2", "3", "4", "{5, 6}"};
        var alphabet = new HashSet<string> {"a", "b"};
        var startState = "{0, 1}";
        var terminateStates = new HashSet<string> {"4", "{5, 6}"};
        var transitions = new HashSet<Tuple<string, string, string>>
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

        var expected = new DFA(states, alphabet, transitions, startState, terminateStates);
        var actual = AlgorithmResolver.GetService<MinimizationAlgorithm>().Get(dfa);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void DeterminizationAlgorithm_Get_CorrectDfa()
    {
        var states = new HashSet<string> {"0", "1", "Ø", "{0, 2}", "{0, 1}"};
        var alphabet = new HashSet<string> {"a", "b"};
        var startState = "0";
        var terminateStates = new HashSet<string> {"0", "{0, 1}", "{0, 2}"};
        var transitions = new HashSet<Tuple<string, string, string>>
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
        var expected = new DFA(states, alphabet, transitions, startState, terminateStates);
        var actual = AlgorithmResolver.GetService<DeterminizationAlgorithm>().Get(ndfa);
        Assert.IsTrue(expected.Equals(actual));
    }

    [Test]
    public void NdfaIsNotDfa()
    {
        Assert.IsFalse(Automata.IsDfa(ndfa.Transitions, ndfa.Alphabet, ndfa.States));
    }
    
    [Test]
    public void DfaIsDfa()
    {
        Assert.IsTrue(Automata.IsDfa(dfa.Transitions, dfa.Alphabet, dfa.States));
    }

    [Test]
    public void Extentions_CountCompoundSets()
    {
        var states = new HashSet<string> {"0", "1", "Ø", "{0, 2}", "{0, 1}"};
        Assert.AreEqual(2, states.CountCompoundSets());
    }

    [Test]
    public void Extentions_HashSetToString_OnMultiElementSet()
    {
        var set = new HashSet<string> { "1", "2", "3" };
        Assert.AreEqual("{1, 2, 3}", set.SetToString());
    }

    [Test]
    public void Extentions_HashSetToString_OnSingleElementSet()
    {
        var set = new HashSet<string> { "1" };
        Assert.AreEqual("1", set.SetToString());
    }

    [Test]
    public void Extentions_StringToHashSet()
    {
        var str = "{1, 2, 3}";
        Assert.AreEqual(new HashSet<string> { "1", "2", "3"}, str.StringToSet());
    }
}