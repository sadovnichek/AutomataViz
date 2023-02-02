namespace Infrastructure;

public class TexFile
{
    private readonly string _filename;

    private const string Header = @"\documentclass{article}" + "\n" +
                                  @"\usepackage[T2A]{fontenc}" + "\n" +
                                  @"\usepackage[a4paper, total={6in, 8in}]{geometry}" + "\n" +
                                  @"\begin{document}" + "\n";
                                  
    private const string Delimiter = "\n" + @"\\";

    public TexFile(string filename)
    {
        _filename = filename;
        File.WriteAllText(filename, Header);
    }
    
    public void Write(string content)
    {
        File.AppendAllText(_filename, content + string.Join("", Enumerable.Repeat(Delimiter, 1)));
    }

    public void WriteWhiteSpace(int spaceNumber)
    {
        File.AppendAllText(_filename, string.Join("", Enumerable.Repeat(Delimiter, spaceNumber))+ "\n");
    }

    public void Close()
    {
        File.AppendAllText(_filename, @"\end{document}");
    }

    public void Delete()
    {
        File.Delete(_filename);
    }
}