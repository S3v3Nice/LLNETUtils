﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LLNETUtils.Configuration.Serialization;

internal class PropertiesConfigSerializer : IConfigSerializer
{
    public ConfigSection Deserialize(string data)
    {
        ConfigDictionary dictionary = new();

        foreach (string line in data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            if (line[0] == '#')
            {
                continue;
            }

            Match match = Regex.Match(line, @"^[a-zA-Z0-9\-_.]+=(?=.+)");
            if (!match.Success)
            {
                continue;
            }

            int splitIndex = match.Index + match.Length - 1;
            string key = line.Substring(0, splitIndex);
            string value = line.Substring(splitIndex + 1);

            dictionary[key] = ParseValue(value);
        }

        return new ConfigSection(dictionary);
    }

    public string Serialize(ConfigSection section)
    {
        StringBuilder content = new StringBuilder("# Properties Config file\r\n# ")
            .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            .Append("\r\n");

        foreach (var entry in section)
        {
            string key = entry.Key;
            object value = entry.Value;

            if (value is bool b)
            {
                value = b ? "true" : "false";
            }

            if (value is double d)
            {
                NumberFormatInfo nfi = new() {NumberDecimalSeparator = "."};
                value = d.ToString(nfi);
            }

            content.Append(key).Append('=').Append(value).Append("\r\n");
        }

        return content.ToString();
    }

    private object ParseValue(string value)
    {
        switch (value.ToLower())
        {
            case "true":
            case "on":
            case "yes":
                return true;
            case "false":
            case "off":
            case "no":
                return false;
            default:
                if (int.TryParse(value, out int intVal))
                {
                    return intVal;
                }

                if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture, out double doubleVal))
                {
                    return doubleVal;
                }

                return value;
        }
    }
}