using Infrastructure;

namespace AutomataCore.Task;

public interface ITask
{
    void Create(TexFile studentPaper, TexFile teacherPaper);
}