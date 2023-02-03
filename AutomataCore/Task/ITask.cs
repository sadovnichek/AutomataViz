using Infrastructure;

namespace AutomataCore.Task;

public interface ITask
{
    string Name { get; }

    ITask Configure(string description, int states, HashSet<string> alphabet);
    
    void Create(TexFile studentPaper, TexFile teacherPaper);
}