using AutomataCore.Automata;
using NUnit.Framework;

namespace AutomataTest;

[TestFixture]
public class Tests
{
    private DFA dfa;
    private NDFA ndfa;

    [SetUp]
    public void SetUp()
    {
        var states = new HashSet<string> {"1", "2"};
        var alphabet = new HashSet<string> {"a", "b"};
        var terminates = new HashSet<string> {"2"};
        dfa = new DFA(states, alphabet, "1", terminates);
        dfa.AddTransition("1", "a", "2");
        dfa.AddTransition("1", "b", "1");
        dfa.AddTransition("2", "a", "2");
        dfa.AddTransition("2", "b", "1");

        ndfa = new NDFA(states, alphabet, "1", terminates);
        ndfa.AddTransition("1", "a", "2");
        ndfa.AddTransition("2", "b", "1");
    }
    
    [Test]
    public void AcceptWord_DFA()
    {
        Assert.AreEqual(true, dfa.IsAcceptWord("a"));
        Assert.AreEqual(true, dfa.IsAcceptWord("aaa"));
        Assert.AreEqual(true, dfa.IsAcceptWord("aabaa"));
        Assert.AreEqual(false, dfa.IsAcceptWord("aab"));
    }
    
    [Test]
    public void AcceptWord_NDFA()
    {
        Assert.AreEqual(true, ndfa.IsAcceptWord("a"));
        Assert.AreEqual(false, ndfa.IsAcceptWord("aaa"));
        Assert.AreEqual(true, ndfa.IsAcceptWord("aba"));
        Assert.AreEqual(false, ndfa.IsAcceptWord("ab"));
    }
}