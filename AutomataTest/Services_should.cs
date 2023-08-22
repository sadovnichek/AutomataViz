using Domain.Services;
using Domain.Automatas;
using NUnit.Framework;
using Infrastructure;

namespace AutomataTest;

[TestFixture]
public class Services_should
{
    private DFA dfa;
    private NDFA ndfa;
    private LambdaNDFA lambdaNdfa;
    private IServiceResolver serviceResolver;

    public void ConfigureDfa()
    {
        var states = new Set{"0", "1", "2", "3", "4", "5", "6"};
        var alphabet = new Set {"a", "b"};
        var terminates = new Set{"4", "5", "6"};
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

    public void ConfigureNdfa()
    {
        var states = new Set {"0", "1", "2"};
        var alphabet = new Set{"a", "b"};
        var terminates = new Set {"0"};
        ndfa = new NDFA(states, alphabet, "0", terminates);
        ndfa.AddTransition("0", "a", "1")
            .AddTransition("1", "b", "0")
            .AddTransition("1", "b", "2")
            .AddTransition("2", "a", "0");
    }
    
    public void ConfigureLambdaNDFA()
    {
        var states = new HashSet<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
        var alphabet = new HashSet<string> { "a", "b", "c" };
        var startState = "1";
        var terminateStates = new HashSet<string> { "12" };
        lambdaNdfa = new LambdaNDFA(states, alphabet, startState, terminateStates);
            lambdaNdfa.AddTransition("1", LambdaNDFA.Lambda, "2")
            .AddTransition("1", LambdaNDFA.Lambda, "3")
            .AddTransition("2", "a", "4")
            .AddTransition("3", "b", "5")
            .AddTransition("4", LambdaNDFA.Lambda, "6")
            .AddTransition("5", LambdaNDFA.Lambda, "6")
            .AddTransition("6", LambdaNDFA.Lambda, "1")
            .AddTransition("6", LambdaNDFA.Lambda, "7")
            .AddTransition("7", LambdaNDFA.Lambda, "8")
            .AddTransition("7", LambdaNDFA.Lambda, "9")
            .AddTransition("8", "b", "10")
            .AddTransition("9", "c", "11")
            .AddTransition("10", LambdaNDFA.Lambda, "12")
            .AddTransition("11", LambdaNDFA.Lambda, "12");
    }

    [OneTimeSetUp]
    public void SetUp()
    {
        ConfigureDfa();
        ConfigureNdfa();
        ConfigureLambdaNDFA();
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
        var states = new Set{"{0, 1}", "2", "3", "4", "{5, 6}"};
        var alphabet = new Set {"a", "b"};
        var startState = "{0, 1}";
        var terminateStates = new Set {"4", "{5, 6}"};
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
        var states = new Set {"0", "1", "Ø", "{0, 2}", "{0, 1}"};
        var alphabet = new Set {"a", "b"};
        var startState = "0";
        var terminateStates = new Set {"0", "{0, 1}", "{0, 2}"};
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

    [Test]
    public void GetNDFA_FromLambdaNDFA()
    {
        var expected = new DFA(
            new Set { "0", "1", "2", "3", "4", "Ø" },
            new Set { "a", "b", "c" },
            "0",
            new Set { "3", "4" });
        expected.AddTransition("0", "a", "1")
            .AddTransition("0", "b", "2")
            .AddTransition("0", "c", "Ø")
            .AddTransition("1", "a", "1")
            .AddTransition("1", "b", "3")
            .AddTransition("1", "c", "4")
            .AddTransition("2", "a", "1")
            .AddTransition("2", "b", "3")
            .AddTransition("2", "c", "4")
            .AddTransition("3", "a", "1")
            .AddTransition("3", "b", "3")
            .AddTransition("3", "c", "4")
            .AddTransition("4", "a", "Ø")
            .AddTransition("4", "b", "Ø")
            .AddTransition("4", "c", "Ø")
            .AddTransition("Ø", "a", "Ø")
            .AddTransition("Ø", "b", "Ø")
            .AddTransition("Ø", "c", "Ø");
        var text = lambdaNdfa.GetTransitionTableFormatted();
        var actual = serviceResolver.GetService<LambdaClosureAlgorithm>().Get(lambdaNdfa);
        Assert.AreEqual(expected, actual);
    }
}