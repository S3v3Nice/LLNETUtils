using System.Collections;
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

    public bool Contains(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    public object? Get(string key)
    {
        return Get<object>(key);
    }

    public T? Get<T>(string key, T? defaultValue = default)
    {
        if (_dictionary.ContainsKey(key))
        {
            return (T) _dictionary[key];
        }

        string[] keys = key.Split(".", 2);

        if (!_dictionary.ContainsKey(keys[0]))
        {
            return defaultValue;
        }

        if (_dictionary[keys[0]] is ConfigSection section)
        {
            return section.Get(keys[1], defaultValue);
        }

        return defaultValue;
    }

    public void Set(string key, object value)
    {
        if (_dictionary.ContainsKey(key))
        {
            _dictionary[key] = value;
            return;
        }

        string[] keys = key.Split(".", 2);
        if (keys.Length > 1)
        {
            ConfigSection childSection;
            if (_dictionary.TryGetValue(keys[0], out object? v) && v is ConfigSection section)
            {
                childSection = section;
            }
            else
            {
                childSection = new ConfigSection();
                _dictionary.Add(keys[0], childSection);
            }

            childSection.Set(keys[1], value);
        }
        else
        {
            _dictionary[keys[0]] = value;
        }
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