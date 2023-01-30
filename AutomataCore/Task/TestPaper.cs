using Automata.Infrastructure;

namespace Automata.Task;

public class TestPaper
{
    private readonly int number;
    private readonly List<ITask> tasks;
    private readonly TexFile studentFile;
    private readonly TexFile teacherFile;
    private readonly bool withSolution;

    private TestPaper(int number, string name, bool withSolution)
    {
        this.number = number;
        tasks = new List<ITask>();
        studentFile = new TexFile($"{name}");
        teacherFile = new TexFile($"{name.Replace(".tex", "_solutions.tex")}");
        this.withSolution = withSolution;
    }

    public static TestPaper Create(int variantsNumber, string filename, bool withSolution = true)
    {
        return new TestPaper(variantsNumber, filename, withSolution);
    }
    
    public TestPaper AddTask(ITask task)
    {
        tasks.Add(task);
        return this;
    }

    public void Generate()
    {
        for (var variant = 1; variant <= number; variant++)
        {
            studentFile.Write(@"\textbf{Вариант " + variant + "}");
            teacherFile.Write(@"\textbf{Вариант " + variant + "}");
            foreach (var task in tasks)
            {
                task.Create(studentFile, teacherFile);
            }
            teacherFile.WriteWhiteSpace(3);
            studentFile.WriteWhiteSpace(3);
        }
        studentFile.Close();
        teacherFile.Close();
        if (!withSolution)
        {
            teacherFile.Delete();
        }
    }
}