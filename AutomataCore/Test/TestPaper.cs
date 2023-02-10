using Infrastructure;

namespace AutomataCore.Test;

public class TestPaper
{
    private readonly int amount;
    private readonly List<IAutomataTask> problems;
    private readonly TexFile studentFile;
    private readonly TexFile teacherFile;

    private TestPaper(int amount, string filename)
    {
        this.amount = amount;
        problems = new List<IAutomataTask>();
        studentFile = new TexFile($"{filename}");
        teacherFile = new TexFile($"{filename.Replace(".tex", "_solutions.tex")}");
    }

    public static TestPaper Create(int amount, string filename)
    {
        return new TestPaper(amount, filename);
    }

    public TestPaper AddTask(IAutomataTask automataTask)
    {
        problems.Add(automataTask);
        return this;
    }

    public void Generate()
    {
        for (var variant = 1; variant <= amount; variant++)
        {
            TexFile.WriteMany(@"\textbf{Вариант " + variant + "}", studentFile, teacherFile);
            foreach (var problem in problems)
            {
                problem.Create(studentFile, teacherFile);
            }

            teacherFile.WriteWhiteSpace(3);
            studentFile.WriteWhiteSpace(3);
        }

        studentFile.Close();
        teacherFile.Close();
    }
}