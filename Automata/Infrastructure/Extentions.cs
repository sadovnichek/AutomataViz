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

    public static HashSet<T> GetRandomSubset<T>(this IEnumerable<T> source, int count)
    {
        if (count > source.Count())
        {
            throw new ArgumentException("Count of result set cannot be more than source set");
        }
        var result = new HashSet<T>();
        var random = new Random();
        while (result.Count < count)
        {
            result.Add(source.ToList()[random.Next(0, source.Count() - 1)]);
        }
        return result;
    }
}