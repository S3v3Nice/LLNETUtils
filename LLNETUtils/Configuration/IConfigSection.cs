using System.Diagnostics.CodeAnalysis;
using LLNETUtils.Utils;

namespace LLNETUtils.Configuration;

public interface IConfigSection : IEnumerable<KeyValuePair<string, object>>
{
    internal LinkedDictionary<string, object> Dictionary { get; set; }

    void Clear();
    
    bool Contains(string key);

    void Remove(string key);
    
    void Set(string key, object value);

    bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value);

    T? Get<T>(string key, T? defaultValue = default);
    
    object? Get(string key);

    string GetString(string key, string defaultValue = "");

    int GetInt(string key, int defaultValue = default);

    double GetFloat(string key, float defaultValue = default);

    double GetDouble(string key, double defaultValue = default);

    bool GetBool(string key, bool defaultValue = default);

    DateTime GetDateTime(string key, DateTime defaultValue = default);

    List<object>? GetList(string key, List<object>? defaultValue = null);

    List<T>? GetList<T>(string key, List<T>? defaultValue = null);

    IConfigSection? GetSection(string key, IConfigSection? defaultValue = null);

    internal bool Find(string key, out IDictionary<string, object> lastDict, out string lastKey);
}