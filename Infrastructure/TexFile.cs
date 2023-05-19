namespace Infrastructure;

public class TexFile
{
    private readonly string filename;
    private object locker = new();
    private static object staticLocker = new();

    private const string Header = @"\documentclass{article}" + "\n" +
                                  @"\usepackage[T2A]{fontenc}" + "\n" +
                                  @"\usepackage[a4paper, total={6in, 8in}]{geometry}" + "\n" +
                                  @"\begin{document}" + "\n";
                                  
    private const string Delimiter = "\n" + @"\\";

    public TexFile(string filename)
    {
        this.filename = filename;
        File.WriteAllText(filename, Header);
    }
    
    public void Write(string content)
    {
        lock (locker)
        {
            File.AppendAllText(filename, content + string.Join("", Enumerable.Repeat(Delimiter, 1)));
        }
    }

    public void WriteWhiteSpace(int spaceNumber)
    {
        lock (locker)
        {
            File.AppendAllText(filename, string.Join("", Enumerable.Repeat(Delimiter, spaceNumber)) + "\n");
        }
    }

    public void Close()
    {
        lock (locker)
        {
            File.AppendAllText(filename, @"\end{document}");
        }
    }

    public static void WriteMany(string text, params TexFile[] files)
    {
        lock (staticLocker)
        {
            foreach (var file in files)
            {
                file.Write(text);
            }
        }
    }
}