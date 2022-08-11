﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LLNETUtils.Utils;

namespace LLNETUtils.Configuration;

public class ConfigSection : IConfigSection
{
    private LinkedDictionary<string, object> _dictionary = new();

    public ConfigSection()
    {
    }

    public ConfigSection(IDictionary<string, object> dictionary)
    {
        foreach (var pair in dictionary)
        {
            object value = pair.Value;

            if (value is IDictionary<string, object> dict)
            {
                value = new ConfigSection(dict);
            }
            else if (value is IEnumerable<object> list)
            {
                value = MakeConfigList(list);
            }

            _dictionary.Add(pair.Key, value);
        }
    }

    LinkedDictionary<string, object> IConfigSection.Dictionary
    {
        get => _dictionary;
        set => _dictionary = value;
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    public bool Contains(string key)
    {
        return ((IConfigSection) this).Find(key, out _, out _);
    }

    public void Remove(string key)
    {
        if (((IConfigSection) this).Find(key, out var lastDict, out string lastKey))
        {
            lastDict.Remove(lastKey);
        }
    }

    public void Set(string key, object value)
    {
        if (((IConfigSection) this).Find(key, out var lastDict, out string lastKey))
        {
            lastDict[lastKey] = value;
            return;
        }

        string[] keys = lastKey.Split('.');
        for (int i = 0; i < keys.Length - 1; i++)
        {
            IConfigSection newSection = new ConfigSection();
            lastDict[keys[i]] = newSection;
            lastDict = newSection.Dictionary;
        }

        lastDict[keys[^1]] = value;
    }

    public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
    {
        if (((IConfigSection) this).Find(key, out var lastDict, out string lastKey) && lastDict[lastKey] is T v)
        {
            value = v;
            return true;
        }

        value = default;
        return false;
    }

    public T? Get<T>(string key, T? defaultValue = default)
    {
        return TryGet(key, out T? result) ? result : defaultValue;
    }
    
    public object? Get(string key)
    {
        return Get<object>(key);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return Get(key, defaultValue)!;
    }

    public int GetInt(string key, int defaultValue = default)
    {
        return Get(key) is IConvertible value ? value.ToInt32(null) : defaultValue;
    }

    public double GetFloat(string key, float defaultValue = default)
    {
        return Get(key) is IConvertible value ? value.ToSingle(null) : defaultValue;
    }

    public double GetDouble(string key, double defaultValue = default)
    {
        return Get(key) is IConvertible value ? value.ToDouble(null) : defaultValue;
    }

    public bool GetBool(string key, bool defaultValue = default)
    {
        return Get(key, defaultValue);
    }

    public DateTime GetDateTime(string key, DateTime defaultValue = default)
    {
        return Get(key, defaultValue);
    }

    public List<object>? GetList(string key, List<object>? defaultValue = null)
    {
        return Get(key, defaultValue);
    }

    public List<T>? GetList<T>(string key, List<T>? defaultValue = null)
    {
        var list = GetList(key);
        return list == null ? defaultValue : list.Cast<T>().ToList();
    }

    public IConfigSection? GetSection(string key, IConfigSection? defaultValue = null)
    {
        return Get(key, defaultValue);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool IConfigSection.Find(string key, out IDictionary<string, object> lastDict, out string lastKey)
    {
        lastDict = _dictionary;
        lastKey = key;
        
        if (_dictionary.TryGetValue(key, out _))
        {
            return true;
        }

        int index = 0;
        while ((index = key.IndexOf('.', index + 1)) != -1)
        {
            string key1 = key[..index];
            string key2 = key[(index + 1)..];

            if (_dictionary.TryGetValue(key1, out object? value) && value is IConfigSection section 
                                                                 && section.Find(key2, out lastDict, out lastKey))
            {
                return true;
            }
        }

        return false;
    }

    private static List<object> MakeConfigList(IEnumerable<object> source)
    {
        List<object> result = new();

        foreach (object item in source)
        {
            if (item is IDictionary<string, object> dict)
            {
                result.Add(new ConfigSection(dict));
            }
            else if (item is IEnumerable<object> list)
            {
                result.Add(MakeConfigList(list));
            }
            else
            {
                result.Add(item);
            }
        }

        return result;
    }
}