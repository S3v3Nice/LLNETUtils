﻿using System.Collections;

namespace LLNETUtils.Configuration;

internal class ConfigDictionary : IDictionary<string, object>
{
    private readonly Dictionary<string, LinkedListNode<KeyValuePair<string, object>>> _dict = new();
    private readonly LinkedList<KeyValuePair<string, object>> _list = new();

    public ConfigDictionary()
    {
    }

    public ConfigDictionary(IDictionary<string, object> other)
    {
        foreach (var pair in other)
        {
            SetConfigItem(pair.Key, pair.Value);
        }
    }

    public object this[string c]
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
                var node = new LinkedListNode<KeyValuePair<string, object>>(keyValuePair);
                _dict[c] = node;
                _list.AddLast(node);
            }
        }
    }

    public ICollection<string> Keys => _list.Select(x => x.Key).ToList();
    public ICollection<object> Values => _list.Select(x => x.Value).ToList();
    public int Count => _dict.Count;
    public bool IsReadOnly => false;

    public bool Contains(KeyValuePair<string, object> item)
    {
        return TryGetValue(item.Key, out object t) && Equals(item.Value, t);
    }

    public bool ContainsKey(string k)
    {
        return _dict.ContainsKey(k);
    }

    public bool TryGetValue(string key, out object value)
    {
        if (ContainsKey(key))
        {
            value = _dict[key].Value.Value;
            return true;
        }

        value = default!;
        return false;
    }

    public void Add(KeyValuePair<string, object> item)
    {
        if (ContainsKey(item.Key))
        {
            return;
        }

        var node = new LinkedListNode<KeyValuePair<string, object>>(item);
        _dict[item.Key] = node;
        _list.AddLast(node);
    }

    public void Add(string key, object value)
    {
        if (ContainsKey(key))
        {
            return;
        }

        Add(new KeyValuePair<string, object>(key, value));
    }

    public void SetConfigItem(string key, object value)
    {
        this[key] = GetConfigItem(value);
    }

    public bool Remove(string key)
    {
        if (!_dict.ContainsKey(key))
        {
            return false;
        }

        _list.Remove(_dict[key]);
        _dict.Remove(key);
        return true;
    }

    public bool Remove(KeyValuePair<string, object> item)
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

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        foreach (var pair in _list)
        {
            array[arrayIndex++] = new KeyValuePair<string, object>(pair.Key, pair.Value);
        }
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Dictionary<string, object> ToDictionary()
    {
        return _list.ToDictionary(pair => pair.Key, pair => GetSimpleItem(pair.Value));
    }

    private object GetSimpleItem(object value)
    {
        switch (value)
        {
            case ConfigSection section:
                return section.ToDictionary();
            case List<object> list:
                return list.Select(GetSimpleItem).ToList();
            default:
                return value;
        }
    }

    private object GetConfigItem(object value)
    {
        switch (value)
        {
            case IDictionary<string, object> dictionary:
                return new ConfigSection(dictionary);
            case IEnumerable<object> collection:
                return collection.Select(GetConfigItem).ToList();
            case Config config:
                return config.Root;
            default:
                return value;
        }
    }
}