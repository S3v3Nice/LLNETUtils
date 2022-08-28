using LLNETUtils.Configuration;

namespace LLNETUtils.Utils;

internal static class ConfigUtils
{
    public static object GetNewConfigItem(object item)
    {
        if (item is IDictionary<string, object> dict && dict is not ConfigDictionary)
        {
            return new ConfigDictionary(dict);
        }

        if (item is IEnumerable<object> list && list is not ConfigList)
        {
            return new ConfigList(list);
        }

        return item;
    }
}