namespace Infrastructure;

public class TexFile
{
    private readonly string filename;

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
        File.AppendAllText(filename, content + string.Join("", Enumerable.Repeat(Delimiter, 1)));
    }

    public void WriteWhiteSpace(int spaceNumber)
    {
        File.AppendAllText(filename, string.Join("", Enumerable.Repeat(Delimiter, spaceNumber)) + "\n");
    }

    public void Close()
    {
        File.AppendAllText(filename, @"\end{document}");
    }

    public void Delete()
    {
        File.Delete(filename);
    }
    
    public static void WriteMany(string text, params TexFile[] files)
    {
        foreach (var file in files)
        {
            file.Write(text);
        }
    }
}