using Infrastructure;

namespace AutomataCore.Task;

public interface IAutomataTask
{
    string Name { get; }

    IAutomataTask Configure(string description, int states, HashSet<string> alphabet);
    
    void Create(TexFile studentPaper, TexFile teacherPaper);
}