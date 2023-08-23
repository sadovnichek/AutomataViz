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
        dfa = Automata.Builder
            .SetStartState("0")
            .SetTerminateStates("4", "5", "6")
            .AddTransition("0", "a", "5")
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
            .AddTransition("6", "b", "1")
            .BuildDFA();
    }

    public void ConfigureNdfa()
    {
        ndfa = Automata.Builder
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
        lambdaNdfa = Automata.Builder
            .SetStartState("1")
            .SetTerminateStates("12")
            .AddTransition("1", LambdaNDFA.Lambda, "2")
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
            .AddTransition("11", LambdaNDFA.Lambda, "12")
            .BuildLambdaNDFA();
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
        var expected = Automata.Builder
            .SetStartState("{0, 1}")
            .SetTerminateStates("4", "{5, 6}")
            .AddTransition("{0, 1}", "a", "{5, 6}")
            .AddTransition("{0, 1}", "b", "2")
            .AddTransition("2", "a", "{0, 1}")
            .AddTransition("2", "b", "4")
            .AddTransition("3", "a", "3")
            .AddTransition("3", "b", "{5, 6}")
            .AddTransition("4", "a", "{5, 6}")
            .AddTransition("4", "b", "2")
            .AddTransition("{5, 6}", "a", "3")
            .AddTransition("{5, 6}", "b", "{0, 1}")
            .BuildDFA();

        var actual = serviceResolver.GetService<MinimizationAlgorithm>().Get(dfa);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void DeterminizationAlgorithm_Get_CorrectDfa()
    {
        var expected = Automata.Builder
            .SetStartState("0")
            .SetTerminateStates("0", "{0, 1}", "{0, 2}")
            .AddTransition("0", "a", "1").AddTransition("0", "b", "Ø")
            .AddTransition("1", "a", "Ø").AddTransition("1", "b", "{0, 2}")
            .AddTransition("Ø", "a", "Ø").AddTransition("Ø", "b", "Ø")
            .AddTransition("{0, 2}", "a", "{0, 1}")
            .AddTransition("{0, 2}", "b", "Ø")
            .AddTransition("{0, 1}", "a", "1")
            .AddTransition("{0, 1}", "b", "{0, 2}")
            .BuildDFA();

        var actual = serviceResolver.GetService<DeterminizationAlgorithm>().Get(ndfa);

        Assert.IsTrue(expected.Equals(actual));
    }

    [Test]
    public void GetNDFA_FromLambdaNDFA()
    {
        var expected = Automata.Builder
            .SetStartState("0")
            .SetTerminateStates("3", "4")
            .AddTransition("0", "a", "1").AddTransition("0", "b", "2")
            .AddTransition("0", "c", "Ø").AddTransition("1", "a", "1")
            .AddTransition("1", "b", "3").AddTransition("1", "c", "4")
            .AddTransition("2", "a", "1").AddTransition("2", "b", "3")
            .AddTransition("2", "c", "4").AddTransition("3", "a", "1")
            .AddTransition("3", "b", "3").AddTransition("3", "c", "4")
            .AddTransition("4", "a", "Ø").AddTransition("4", "b", "Ø")
            .AddTransition("4", "c", "Ø").AddTransition("Ø", "a", "Ø")
            .AddTransition("Ø", "b", "Ø").AddTransition("Ø", "c", "Ø")
            .BuildDFA();
        var actual = serviceResolver.GetService<LambdaClosureAlgorithm>().Get(lambdaNdfa);
        Assert.AreEqual(expected, actual);
    }
}