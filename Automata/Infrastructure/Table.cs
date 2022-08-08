using System.Collections;

namespace Automata.Infrastructure;

public class Table<TRow, TColumn, TValue> : IEnumerable<Tuple<TRow, TColumn>>
{
    public HashSet<TRow> Rows { get; }
    public HashSet<TColumn> Columns { get; }
    private Dictionary<Tuple<TRow, TColumn>, TValue> _table;

    public Table()
    {
        Rows = new HashSet<TRow>();
        Columns = new HashSet<TColumn>();
        _table = new Dictionary<Tuple<TRow, TColumn>, TValue>();
    }

    public TValue this[TColumn columnName, TRow rowName]
    {
        get => _table[Tuple.Create(rowName, columnName)];
        set
        {
            _table[Tuple.Create(rowName, columnName)] = value;
            Columns.Add(columnName);
            Rows.Add(rowName);
        }
    }

    public IEnumerator<Tuple<TRow, TColumn>> GetEnumerator()
    {
        return _table.Select(pair => Tuple.Create(pair.Key.Item1, pair.Key.Item2)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}