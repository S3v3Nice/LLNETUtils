using System.Collections;

namespace LLNETUtils.Utils;

public class LinkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _dict = new();
    private readonly LinkedList<KeyValuePair<TKey, TValue>> _list = new();

    public TValue this[TKey c]
    {
        get => _dict[c].Value.Value;

        set
        {
            var keyValuePair = KeyValuePair.Create(c, value);
            
            if (_dict.ContainsKey(c))
            {
                _dict[c].Value = keyValuePair;
            }
            else
            {
                var node = new LinkedListNode<KeyValuePair<TKey, TValue>>(keyValuePair);
                _dict[c] = node;
                _list.AddLast(node);
            }
        }
    }

    public ICollection<TKey> Keys => _list.Select(x => x.Key).ToList();
    public ICollection<TValue> Values => _list.Select(x => x.Value).ToList();
    public int Count => _dict.Count;
    public bool IsReadOnly => false;

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return TryGetValue(item.Key, out TValue t) && Equals(item.Value, t);
    }

    public bool ContainsKey(TKey k)
    {
        return _dict.ContainsKey(k);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (ContainsKey(key))
        {
            value = _dict[key].Value.Value;
            return true;
        }

        value = default!;
        return false;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if (ContainsKey(item.Key))
        {
            return;
        }

        var node = new LinkedListNode<KeyValuePair<TKey, TValue>>(item);
        _dict[item.Key] = node;
        _list.AddLast(node);
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            return;
        }

        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool Remove(TKey key)
    {
        if (!_dict.ContainsKey(key))
        {
            return false;
        }

        _list.Remove(_dict[key]);
        _dict.Remove(key);
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var node = _list.Find(item);

        if (node == null)
        {
            return false;
        }

        _dict.Remove(item.Key);
        _list.Remove(node);
        return true;
    }

    public void Clear()
    {
        _dict.Clear();
        _list.Clear();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var pair in _list)
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
        }
    }


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}