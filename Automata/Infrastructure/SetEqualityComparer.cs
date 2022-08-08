namespace Automata.Infrastructure;

public class SetEqualityComparer : IEqualityComparer<HashSet<HashSet<string>>>
{
    public bool Equals(HashSet<HashSet<string>> x, HashSet<HashSet<string>> y)
    {
        if (x.Any(set => !y.Contains(set)) || y.Any(set => !x.Contains(set)))
        {
            return false;
        }
        return true;
    }

    public int GetHashCode(HashSet<HashSet<string>> obj)
    {
        unchecked
        {
            var hashcode = obj.First().First().GetHashCode();
            return obj.SelectMany(set => set)
                .Aggregate(hashcode, (current, element) => (current * 397) ^ element.GetHashCode());
        }
    }
}