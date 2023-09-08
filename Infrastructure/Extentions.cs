namespace Infrastructure;

public static class Extensions
{
    public static string EmptySet => "Ø";

    public static string SetToString(this HashSet<string> source, bool latex = false)
    {
        if (!source.Any())
            return EmptySet;
        if (source.Count == 1)
            return source.First();
        if (!latex) 
            return @"{" + string.Join(", ", source.ToSortedSet()) + "}";
        var compounds = source.Select(x => x.StringToSet()).Select(x => x.SetToString(true));
        return @"\{" + string.Join(", ", compounds.ToSortedSet()) + @"\}";
    }

    public static HashSet<string> StringToSet(this string source)
    {
        if (source.Length == 0)
            return Enumerable.Empty<string>().ToHashSet();
        return source.Trim('{', '}')
            .Split(", ")
            .ToSortedSet()
            .ToHashSet();
    }

    public static SortedSet<string> ToSortedSet(this IEnumerable<string> source)
    {
        var result = new SortedSet<string>();
        foreach (var element in source)
        {
            result.Add(element);
        }
        return result;
    }
    
    public static HashSet<T> GetRandomSubset<T>(this IEnumerable<T> source, int count)
    {
        if (count > source.Count())
            throw new ArgumentException("Count of result set cannot be more than source set");
        var result = new HashSet<T>();
        var random = new Random();
        while (result.Count < count)
        {
            result.Add(source.ToList()[random.Next(0, source.Count() - 1)]);
        }
        return result;
    }

    public static int CountCompoundSets(this IEnumerable<string> source)
    {
        return source.Count(s => s.StringToSet().Count > 1);
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T element)
    {
        return source.Except(new [] { element });
    }
}