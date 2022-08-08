namespace Automata.Infrastructure;

public static class Extensions
{
    public static string SetToString<T>(this T source)
    {
        return source switch
        {
            IEnumerable<string> set => 
                @"{" + string.Join(", ", set) + "}",
            IEnumerable<IEnumerable<string>> compoundSet => 
                compoundSet.Aggregate("", (current, set) => current + (set.SetToString() + "; ")),
            _ => 
                source.ToString()
        };
    }
}