using LLNETUtils.Utils;

namespace LLNETUtils.Configuration;

public class ConfigSection : LinkedDictionary<string, object>
{
    public object? Get(string key)
    {
        return Get<object>(key);
    }

    public T? Get<T>(string key, T? defaultValue = default)
    {
        if (ContainsKey(key))
        {
            return (T) this[key];
        }

        string[] keys = key.Split(".", 2);

        if (!ContainsKey(keys[0]))
        {
            return defaultValue;
        }

        if (this[keys[0]] is ConfigSection section)
        {
            return section.Get(keys[1], defaultValue);
        }

        return defaultValue;
    }

    public void Set(string key, object value)
    {
        string[] keys = key.Split(".", 2);
        if (keys.Length > 1)
        {
            ConfigSection childSection;
            if (TryGetValue(keys[0], out object? v) && v is ConfigSection section)
            {
                childSection = section;
            }
            else
            {
                childSection = new ConfigSection();
                Add(keys[0], childSection);
            }

            childSection.Set(keys[1], value);
        }
        else
        {
            this[keys[0]] = value;
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

    public ConfigSection? GetSection(string key, ConfigSection? defaultValue = null)
    {
        return Get(key, defaultValue);
    }
}