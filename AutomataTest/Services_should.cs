using Domain.Services;
using Domain.Automatas;
using NUnit.Framework;

namespace AutomataTest;

public class Services_should
{
    private DFA dfa;
    private NDFA ndfa;
    private LambdaNDFA lambdaNdfa;

    public void ConfigureDfa()
    {
        dfa = AutomataBuilder.CreateAutomata()
            .SetStartState("0")
            .SetTerminateStates("4", "5", "6")
            .AddTransition("0", "a", "5").AddTransition("0", "b", "2")
            .AddTransition("1", "a", "6").AddTransition("1", "b", "2")
            .AddTransition("2", "a", "0").AddTransition("2", "b", "4")
            .AddTransition("3", "a", "3").AddTransition("3", "b", "5")
            .AddTransition("4", "a", "6").AddTransition("4", "b", "2")
            .AddTransition("5", "a", "3").AddTransition("5", "b", "0")
            .AddTransition("6", "a", "3").AddTransition("6", "b", "1")
            .BuildDFA();
    }

    public void ConfigureNdfa()
    {
        ndfa = AutomataBuilder.CreateAutomata()
            .SetStartState("0")
            .SetTerminateStates("0")
            .AddTransition("0", "a", "1")
            .AddTransition("1", "b", "0")
            .AddTransition("1", "b", "2")
            .AddTransition("2", "a", "0")
            .BuildNDFA();
    }
    
    public void ConfigureLambdaNDFA()
    {
        lambdaNdfa = AutomataBuilder.CreateAutomata()
            .SetStartState("1")
            .SetTerminateStates("12")
            .AddTransition("1", Automata.Lambda, "2").AddTransition("1", Automata.Lambda, "3")
            .AddTransition("2", "a", "4").AddTransition("3", "b", "5")
            .AddTransition("4", Automata.Lambda, "6").AddTransition("5", Automata.Lambda, "6")
            .AddTransition("6", Automata.Lambda, "1").AddTransition("6", Automata.Lambda, "7")
            .AddTransition("7", Automata.Lambda, "8").AddTransition("7", Automata.Lambda, "9")
            .AddTransition("8", "b", "10").AddTransition("9", "c", "11")
            .AddTransition("10", Automata.Lambda, "12").AddTransition("11", Automata.Lambda, "12")
            .BuildLambdaNDFA();
    }

    [OneTimeSetUp]
    public void SetUp()
    {
        ConfigureDfa();
        ConfigureNdfa();
        ConfigureLambdaNDFA();
    }
    
    [Test]
    [TestCase("a", true)]
    [TestCase("aaa", false)]
    [TestCase("aabaa", false)]
    [TestCase("aab", true)]
    public void Dfa_IsRecognizeWord_Correctly(string word, bool expected)
    {
        var algorithm = new WordRecognitionAlgorithm();

        var actual = algorithm.Get(dfa, word);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase("a", false)]
    [TestCase("aaa", false)]
    [TestCase("aba", true)]
    [TestCase("ab", true)]
    public void Ndfa_IsRecognizeWord_Correctly(string word, bool expected)
    {
        var algorithm = new WordRecognitionAlgorithm();

        var actual = algorithm.Get(ndfa, word);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void MinimizationAlgorithm_Get_CorrectDfa()
    {
        var expected = AutomataBuilder.CreateAutomata()
            .SetStartState("{0, 1}")
            .SetTerminateStates("4", "{5, 6}")
            .AddTransition("{0, 1}", "a", "{5, 6}").AddTransition("{0, 1}", "b", "2")
            .AddTransition("2", "a", "{0, 1}").AddTransition("2", "b", "4")
            .AddTransition("3", "a", "3").AddTransition("3", "b", "{5, 6}")
            .AddTransition("4", "a", "{5, 6}").AddTransition("4", "b", "2")
            .AddTransition("{5, 6}", "a", "3").AddTransition("{5, 6}", "b", "{0, 1}")
            .BuildDFA();
        var algorithm = new MinimizationAlgorithm();

        var actual = algorithm.Get(dfa);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void DeterminizationAlgorithm_Get_CorrectDfa()
    {
        var expected = AutomataBuilder.CreateAutomata()
            .SetStartState("0")
            .SetTerminateStates("0", "{0, 1}", "{0, 2}")
            .AddTransition("0", "a", "1").AddTransition("0", "b", Automata.EmptySet)
            .AddTransition("1", "a", Automata.EmptySet).AddTransition("1", "b", "{0, 2}")
            .AddTransition(Automata.EmptySet, "a", Automata.EmptySet)
            .AddTransition(Automata.EmptySet, "b", Automata.EmptySet)
            .AddTransition("{0, 2}", "a", "{0, 1}").AddTransition("{0, 2}", "b", Automata.EmptySet)
            .AddTransition("{0, 1}", "a", "1").AddTransition("{0, 1}", "b", "{0, 2}")
            .BuildDFA();
        var algorithm = new DeterminizationAlgorithm();

        var actual = algorithm.Get(ndfa);

        Assert.IsTrue(expected.Equals(actual));
    }

    [Test]
    public void GetNDFA_FromLambdaNDFA()
    {
        var expected = AutomataBuilder.CreateAutomata()
            .SetStartState("0")
            .SetTerminateStates("3", "4")
            .AddTransition("0", "a", "1").AddTransition("0", "b", "2")
            .AddTransition("0", "c", Automata.EmptySet).AddTransition("1", "a", "1")
            .AddTransition("1", "b", "3").AddTransition("1", "c", "4")
            .AddTransition("2", "a", "1").AddTransition("2", "b", "3")
            .AddTransition("2", "c", "4").AddTransition("3", "a", "1")
            .AddTransition("3", "b", "3").AddTransition("3", "c", "4")
            .AddTransition("4", "a", Automata.EmptySet).AddTransition("4", "b", Automata.EmptySet)
            .AddTransition("4", "c", Automata.EmptySet)
            .AddTransition(Automata.EmptySet, "a", Automata.EmptySet)
            .AddTransition(Automata.EmptySet, "b", Automata.EmptySet)
            .AddTransition(Automata.EmptySet, "c", Automata.EmptySet)
            .BuildDFA();
        var algorithm = new LambdaClosureAlgorithm();

        var actual = algorithm.Get(lambdaNdfa);

        Assert.AreEqual(expected, actual);
    }
}