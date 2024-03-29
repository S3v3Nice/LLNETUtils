﻿using System.Diagnostics.CodeAnalysis;

namespace LLNETUtils.Configuration;

/// <summary>Interface with methods for reading and editing the config section</summary>
public interface IConfigSection : IEnumerable<KeyValuePair<string, object>>
{
    /// <summary>Removes all items from the config section.</summary>
    void Clear();

    /**
     * <summary>Returns whether the config section contains an item the specified key.</summary>
     * <param name="key">The key to locate in the config section.</param>
     * <returns>true if the config section contains an item with the specified key; otherwise, false.</returns>
     */
    bool Contains(string key);

    /**
     * <summary>Removes a value associated with the specified key.</summary>
     * <param name="key">The key of the value to remove.</param>
     */
    void Remove(string key);

    /**
     * <summary>Sets a value with associated the specified key in the config section.</summary>
     * <param name="key">The key of the value to set.</param>
     * <param name="value">The value to set.</param>
     */
    void Set(string key, object value);

    /**
     * <summary>Returns the value of a certain type associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="value">
     *     When this method returns, contains the value associated with the specified key,
     *     if the key is found; otherwise, the default value for the type of the value parameter.
     * </param>
     * <returns>true if the config section contains an item with the specified key; otherwise, false.</returns>
     */
    bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value);

    /**
     * <summary>Returns the value of a certain type associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>Value of a certain type if the key is found; otherwise, defaultValue.</returns>
     */
    T? Get<T>(string key, T? defaultValue = default);

    /**
     * <summary>Returns the object value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <returns>object value if the key is found; otherwise, null.</returns>
     */
    object? Get(string key);

    /**
     * <summary>Returns the string value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>string value if the key is found; otherwise, defaultValue.</returns>
     */
    string GetString(string key, string defaultValue = "");

    /**
     * <summary>Returns the int value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>int value if the key is found; otherwise, defaultValue.</returns>
     */
    int GetInt(string key, int defaultValue = default);

    /**
     * <summary>Returns the float value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>float value if the key is found; otherwise, defaultValue.</returns>
     */
    float GetFloat(string key, float defaultValue = default);

    /**
     * <summary>Returns the double value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>double value if the key is found; otherwise, defaultValue.</returns>
     */
    double GetDouble(string key, double defaultValue = default);

    /**
     * <summary>Returns the bool value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>bool value if the key is found; otherwise, defaultValue.</returns>
     */
    bool GetBool(string key, bool defaultValue = default);

    /**
     * <summary>Returns the DateTime value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>DateTime value if the key is found; otherwise, defaultValue.</returns>
     */
    DateTime GetDateTime(string key, DateTime defaultValue = default);

    /**
     * <summary>Returns the List&lt;object&gt; value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>List&lt;object&gt; value if the key is found; otherwise, defaultValue.</returns>
     */
    List<object>? GetList(string key, List<object>? defaultValue = null);

    /**
     * <summary>Returns the List&lt;T&gt; value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>List&lt;T&gt; value if the key is found; otherwise, defaultValue.</returns>
     */
    List<T>? GetList<T>(string key, List<T>? defaultValue = null);

    /**
     * <summary>Returns the IConfigSection value associated with the specified key.</summary>
     * <param name="key">The key of the value to return.</param>
     * <param name="defaultValue">
     *     Default value that will be returned if no value
     *     with the specified key is found in the config section.
     * </param>
     * <returns>IConfigSection value if the key is found; otherwise, defaultValue.</returns>
     */
    IConfigSection? GetSection(string key, IConfigSection? defaultValue = null);

    /**
     * <summary>Creates Dictionary&lt;string, object&gt; from ConfigSection</summary>
     */
    Dictionary<string, object> ToDictionary();
}