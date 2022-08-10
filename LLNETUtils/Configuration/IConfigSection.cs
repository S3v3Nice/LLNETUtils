using LLNETUtils.Utils;

namespace LLNETUtils.Configuration;

public interface IConfigSection : IEnumerable<KeyValuePair<string, object>>
{
    internal LinkedDictionary<string, object> Dictionary { get; set; }
    
    bool Contains(string key);
    
    object? Get(string key);

    T? Get<T>(string key, T? defaultValue = default);

    void Set(string key, object value);

    string GetString(string key, string defaultValue = "");

    int GetInt(string key, int defaultValue = default);

    double GetFloat(string key, float defaultValue = default);

    double GetDouble(string key, double defaultValue = default);

    bool GetBool(string key, bool defaultValue = default);

    DateTime GetDateTime(string key, DateTime defaultValue = default);

    List<object>? GetList(string key, List<object>? defaultValue = null);

    List<T>? GetList<T>(string key, List<T>? defaultValue = null);

    IConfigSection? GetSection(string key, IConfigSection? defaultValue = null);
}