using Automata.Infrastructure;

namespace Automata.Algorithm;

public class DeterminizationAlgorithm : IAlgorithm
{
    private const string Lambda = "λ";
    
    private void GetClosure(Table<string, string, string> table, string vertex)
    {
        
    }
    
    public Automata<HashSet<string>> Get(Automata<string> source)
    {
        throw new NotImplementedException();
    }

    public bool IsAppropriate(Automata<string> source, Automata<HashSet<string>> result)
    {
        throw new NotImplementedException();
    }
}