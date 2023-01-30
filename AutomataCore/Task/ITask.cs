using Automata.Infrastructure;

namespace Automata.Task;

public interface ITask
{
    void Create(TexFile studentPaper, TexFile teacherPaper);
}