using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LLNETUtils.Configuration;

/// <summary>Class for reading and editing the config section</summary>
public class ConfigSection : IConfigSection
{
    internal readonly ConfigDictionary Dictionary;

    public ConfigSection()
    {
        Dictionary = new ConfigDictionary();
    }

    /**
     * <param name="dictionary">The dictionary on which ConfigSection will be based.</param>
     */
    public ConfigSection(IDictionary<string, object> dictionary)
    {
        Dictionary = dictionary as ConfigDictionary ?? new ConfigDictionary(dictionary);
    }

    public void Clear()
    {
        Dictionary.Clear();
    }

    public bool Contains(string key)
    {
        return Find(key, out _, out _);
    }

    public void Remove(string key)
    {
        if (Find(key, out var lastDict, out string lastKey))
        {
            lastDict.Remove(lastKey);
        }
    }

    public void Set(string key, object value)
    {
        if (Find(key, out var lastDict, out string lastKey))
        {
            lastDict.SetConfigItem(lastKey, value);
            return;
        }

        string[] keys = lastKey.Split('.');
        for (int i = 0; i < keys.Length - 1; i++)
        {
            ConfigDictionary dictionary = new();
            lastDict.SetConfigItem(keys[i], dictionary);
            lastDict = dictionary;
        }

        lastDict.SetConfigItem(keys[^1], value);
    }

    public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
    {
        if (Find(key, out var lastDict, out string lastKey) && lastDict[lastKey] is T v)
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

    public List<object>? GetList(string key, List<object>? defaultValue = null)
    {
        var list = Get<List<object>>(key);
        return list?.ToList() ?? defaultValue;
    }

    public List<T>? GetList<T>(string key, List<T>? defaultValue = null)
    {
        var list = Get<List<object>>(key);
        return list?.Cast<T>().ToList() ?? defaultValue;
    }

    public IConfigSection? GetSection(string key, IConfigSection? defaultValue = null)
    {
        ConfigDictionary? dictionary = Get<ConfigDictionary>(key);
        return dictionary != null ? new ConfigSection(dictionary) : defaultValue;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return Dictionary.ToDictionary();
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private bool Find(string key, out ConfigDictionary lastDict, out string lastKey)
    {
        lastDict = Dictionary;
        lastKey = key;
        
        if (Dictionary.TryGetValue(key, out _))
        {
            return true;
        }

        int index = 0;
        while ((index = key.IndexOf('.', index + 1)) != -1)
        {
            string key1 = key[..index];
            string key2 = key[(index + 1)..];

            if (Dictionary.TryGetValue(key1, out object? value) && value is ConfigSection section &&
                section.Find(key2, out lastDict, out lastKey))
            {
                return true;
            }
        }

        return false;
    }
}
