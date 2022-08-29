using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace LLNETUtils.Configuration.Serialization;

internal class YamlConfigSerializer : IConfigSerializer
{
    public ConfigSection Deserialize(string data)
    {
        DeserializerBuilder builder = new DeserializerBuilder()
            .WithNodeTypeResolver(new ConfigNodeTypeResolver());
        ConfigDictionary? result = builder.Build().Deserialize<ConfigDictionary?>(data);

        if (result == null)
        {
            throw new ArgumentException("The data provided is not a valid Yaml object!");
        }

        return new ConfigSection(result);
    }

    public string Serialize(ConfigSection section)
    {
        SerializerBuilder builder = new SerializerBuilder()
            .WithEventEmitter(emitter => new ForceQuoteEventEmitter(emitter));

        return builder.Build().Serialize(section.Dictionary);
    }

    private class ForceQuoteEventEmitter : ChainedEventEmitter
    {
        public ForceQuoteEventEmitter(IEventEmitter nextEmitter) : base(nextEmitter)
        {
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.Type == typeof(string) && eventInfo.Source.StaticType == typeof(object))
            {
                eventInfo.Style = ScalarStyle.DoubleQuoted;
            }

            base.Emit(eventInfo, emitter);
        }
    }

    private class ConfigNodeTypeResolver : INodeTypeResolver
    {
        bool INodeTypeResolver.Resolve(NodeEvent? nodeEvent, ref Type currentType)
        {
            if (currentType != typeof(object))
            {
                return false;
            }

            if (nodeEvent is MappingStart)
            {
                currentType = typeof(ConfigDictionary);
                return true;
            }

            if (nodeEvent is Scalar scalar)
            {
                if (scalar.IsQuotedImplicit)
                {
                    currentType = typeof(string);
                    return true;
                }

                if (bool.TryParse(scalar.Value, out bool _))
                {
                    currentType = typeof(bool);
                    return true;
                }

                if (int.TryParse(scalar.Value, out int _))
                {
                    currentType = typeof(int);
                    return true;
                }

                if (double.TryParse(scalar.Value, NumberStyles.Float | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture, out double _))
                {
                    currentType = typeof(double);
                    return true;
                }

                if (DateTime.TryParse(scalar.Value, out DateTime _))
                {
                    currentType = typeof(DateTime);
                    return true;
                }
            }

            return false;
        }
    }
}