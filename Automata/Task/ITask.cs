using Automata.Infrastructure;

namespace Automata.Task;

public interface ITask
{
    string Description { get; }

    void Create(TexFile studentPaper, TexFile teacherPaper);
}