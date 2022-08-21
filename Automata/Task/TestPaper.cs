using Automata.Infrastructure;

namespace Automata.Task;

public class TestPaper
{
    private readonly int _variantsNumber;
    private readonly List<ITask> _tasks;
    private readonly TexFile _studentFile;
    private readonly TexFile _teacherFile;
    private readonly bool _withSolution;

    private TestPaper(int variantsNumber, string name, bool withSolution)
    {
        _variantsNumber = variantsNumber;
        _tasks = new List<ITask>();
        _studentFile = new TexFile($"{name}");
        _teacherFile = new TexFile($"{name.Replace(".tex", "_solutions.tex")}");
        _withSolution = withSolution;
    }

    public static TestPaper Create(int variantsNumber, string filename, bool withSolution = true)
    {
        return new TestPaper(variantsNumber, filename, withSolution);
    }
    
    public TestPaper AddTask(ITask task)
    {
        _tasks.Add(task);
        return this;
    }

    public void Generate()
    {
        for (var variant = 1; variant <= _variantsNumber; variant++)
        {
            _studentFile.Write(@"\textbf{Вариант " + variant + "}");
            _teacherFile.Write(@"\textbf{Вариант " + variant + "}");
            foreach (var task in _tasks)
            {
                task.Create(_studentFile, _teacherFile);
            }
            _teacherFile.WriteWhiteSpace(3);
            _studentFile.WriteWhiteSpace(3);
        }
        _studentFile.Close();
        _teacherFile.Close();
        if (!_withSolution)
        {
            _teacherFile.Delete();
        }
    }
}