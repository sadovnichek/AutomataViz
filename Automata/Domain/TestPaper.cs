using Automata.Infrastructure;

namespace Automata.Domain;

public class TestPaper
{
    private readonly int _number;
    private readonly List<Task> _tasks;
    private readonly TexFile studentFile;
    private readonly TexFile teacherFile;
    private readonly bool _withSolution;

    private TestPaper(int number, string name, bool withSolution)
    {
        _number = number;
        _tasks = new List<Task>();
        studentFile = new TexFile($"{name}.tex");
        teacherFile = new TexFile($"{name}_solutions.tex");
        _withSolution = withSolution;
    }

    public static TestPaper Create(int taskNumber, string filename, bool withSolution = true)
    {
        return new TestPaper(taskNumber, filename, withSolution);
    }
    
    public TestPaper AddTask(Task task)
    {
        _tasks.Add(task);
        return this;
    }

    public void Generate()
    {
        for (var variant = 1; variant <= _number; variant++)
        {
            studentFile.Write(@"\textbf{Вариант " + variant + "}");
            teacherFile.Write(@"\textbf{Вариант " + variant + "}");
            foreach (var task in _tasks)
            {
                task.CreateTask(studentFile, teacherFile);
            }
            teacherFile.WriteWhiteSpace(3);
            studentFile.WriteWhiteSpace(3);
        }
        studentFile.Close();
        teacherFile.Close();
        if (!_withSolution)
        {
            teacherFile.Delete();
        }
    }
}