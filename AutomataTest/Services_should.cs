using Domain.Services;
using Domain.Automatas;
using NUnit.Framework;

namespace AutomataTest;

[TestFixture]
public class Services_should
{
    private DFA dfa;
    private NDFA ndfa;
    private IServiceResolver serviceResolver;

    private void ConfigureDfa()
    {
        var states = new HashSet<string> {"0", "1", "2", "3", "4", "5", "6"};
        var alphabet = new HashSet<string> {"a", "b"};
        var terminates = new HashSet<string> {"4", "5", "6"};
        dfa = new DFA(states, alphabet, "0", terminates);
        dfa.AddTransition("0", "a", "5")
            .AddTransition("0", "b", "2")
            .AddTransition("1", "a", "6")
            .AddTransition("1", "b", "2")
            .AddTransition("2", "a", "0")
            .AddTransition("2", "b", "4")
            .AddTransition("3", "a", "3")
            .AddTransition("3", "b", "5")
            .AddTransition("4", "a", "6")
            .AddTransition("4", "b", "2")
            .AddTransition("5", "a", "3")
            .AddTransition("5", "b", "0")
            .AddTransition("6", "a", "3")
            .AddTransition("6", "b", "1");
    }

    private void ConfigureNdfa()
    {
        var states = new HashSet<string> {"0", "1", "2"};
        var alphabet = new HashSet<string> {"a", "b"};
        var terminates = new HashSet<string> {"0"};
        ndfa = new NDFA(states, alphabet, "0", terminates);
        ndfa.AddTransition("0", "a", "1")
            .AddTransition("1", "b", "0")
            .AddTransition("1", "b", "2")
            .AddTransition("2", "a", "0");
    }
    
    [OneTimeSetUp]
    public void SetUp()
    {
        ConfigureDfa();
        ConfigureNdfa();
        serviceResolver = new ServiceResolver();
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
        var transitions = new HashSet<Transition>
        {
            new("{0, 1}", "a", "{5, 6}"),
            new("{0, 1}", "b", "2"),
            new("2", "a", "{0, 1}"),
            new("2", "b", "4"),
            new("3", "a", "3"),
            new("3", "b", "{5, 6}"),
            new("4", "a", "{5, 6}"),
            new("4", "b", "2"),
            new("{5, 6}", "a", "3"),
            new("{5, 6}", "b", "{0, 1}")
        };

        var expected = new DFA(states, alphabet, transitions, startState, terminateStates);
        var actual = serviceResolver.GetService<MinimizationAlgorithm>().Get(dfa);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void DeterminizationAlgorithm_Get_CorrectDfa()
    {
        var states = new HashSet<string> {"0", "1", "Ø", "{0, 2}", "{0, 1}"};
        var alphabet = new HashSet<string> {"a", "b"};
        var startState = "0";
        var terminateStates = new HashSet<string> {"0", "{0, 1}", "{0, 2}"};
        var transitions = new HashSet<Transition>
        {
            new("0", "a", "1"),
            new("0", "b", "Ø"),
            new("1", "a", "Ø"),
            new("1", "b", "{0, 2}"),
            new("Ø", "a", "Ø"),
            new("Ø", "b", "Ø"),
            new("{0, 2}", "a", "{0, 1}"),
            new("{0, 2}", "b", "Ø"),
            new("{0, 1}", "a", "1"),
            new("{0, 1}", "b", "{0, 2}"),
        };

        var expected = new DFA(states, alphabet, transitions, startState, terminateStates);
        var actual = serviceResolver.GetService<DeterminizationAlgorithm>().Get(ndfa);

        Assert.IsTrue(expected.Equals(actual));
    }

    [Test]
    public void IsDfa_ShouldBeFalse_WhenNdfa()
    {
        Assert.IsFalse(Automata.IsDfa(ndfa.Transitions, ndfa.Alphabet, ndfa.States));
    }
    
    [Test]
    public void IsDfa_ShouldBeTrue_WhenDfa()
    {
        Assert.IsTrue(Automata.IsDfa(dfa.Transitions, dfa.Alphabet, dfa.States));
    }
}