namespace DotnetPromptToolkit.Cache;

/// <summary>Mirrors prompt_toolkit.cache.SimpleCache - a small LRU cache.</summary>
public sealed class SimpleCache<TKey, TValue> where TKey : notnull
{
    private readonly LinkedList<TKey> _order = new();
    private readonly Dictionary<TKey, (LinkedListNode<TKey> Node, TValue Value)> _data = new();
    private readonly int _maxSize;

    public SimpleCache(int maxSize = 8)
    {
        if (maxSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxSize));
        _maxSize = maxSize;
    }

    public int Count => _data.Count;

    public TValue Get(TKey key, Func<TValue> factory)
    {
        if (_data.TryGetValue(key, out var entry))
        {
            _order.Remove(entry.Node);
            _order.AddLast(entry.Node);
            return entry.Value;
        }

        var value = factory();
        var node = _order.AddLast(key);
        _data[key] = (node, value);
        if (_data.Count > _maxSize)
        {
            var oldest = _order.First!;
            _order.RemoveFirst();
            _data.Remove(oldest.Value);
        }
        return value;
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (_data.TryGetValue(key, out var entry))
        {
            value = entry.Value;
            return true;
        }
        value = default!;
        return false;
    }

    public void Clear()
    {
        _order.Clear();
        _data.Clear();
    }
}

/// <summary>Mirrors prompt_toolkit.cache.FastDictCache - dict with a factory and unbounded growth.</summary>
public sealed class FastDictCache<TKey, TValue>(Func<TKey, TValue> factory) where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _data = new();

    public TValue this[TKey key]
    {
        get
        {
            if (!_data.TryGetValue(key, out var value))
            {
                value = factory(key);
                _data[key] = value;
            }
            return value;
        }
    }

    public int Count => _data.Count;
    public void Clear() => _data.Clear();
}
