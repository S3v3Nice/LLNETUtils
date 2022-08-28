using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LLNETUtils.Configuration;

/// <summary>Class for reading and editing the config section</summary>
public class ConfigSection : IConfigSection
{
    private ConfigDictionary _dictionary;

    public ConfigSection()
    {
        _dictionary = new ConfigDictionary();
    }

    /**
     * <param name="dictionary">The dictionary to take as the base for the config section.</param>
     */
    public ConfigSection(IDictionary<string, object> dictionary)
    {
        _dictionary = dictionary as ConfigDictionary ?? new ConfigDictionary(dictionary);
    }

    ConfigDictionary IConfigSection.Dictionary
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
            ConfigDictionary dictionary = new();
            lastDict[keys[i]] = dictionary;
            lastDict = dictionary;
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

    public float GetFloat(string key, float defaultValue = default)
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

    public IList<object>? GetList(string key, IList<object>? defaultValue = null)
    {
        return Get(key, defaultValue);
    }

    public IList<T>? GetList<T>(string key, IList<T>? defaultValue = null)
    {
        var list = GetList(key);
        return list?.Cast<T>().ToList() ?? defaultValue;
    }

    public IDictionary<string, object>? GetDictionary(string key, IDictionary<string, object>? defaultValue = null)
    {
        return Get(key, defaultValue);
    }

    public IConfigSection? GetSection(string key, IConfigSection? defaultValue = null)
    {
        ConfigDictionary? dictionary = Get<ConfigDictionary>(key);
        return dictionary != null ? new ConfigSection(dictionary) : defaultValue;
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

            if (_dictionary.TryGetValue(key1, out object? value) && value is ConfigDictionary dictionary)
            {
                IConfigSection section = new ConfigSection(dictionary);

                if (section.Find(key2, out lastDict, out lastKey))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
