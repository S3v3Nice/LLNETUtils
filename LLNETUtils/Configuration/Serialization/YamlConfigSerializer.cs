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
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNodeTypeResolver(new ConfigNodeTypeResolver())
            .Build();
        return deserializer.Deserialize<ConfigSection>(data);
    }

    public string Serialize(ConfigSection section)
    {
        ISerializer serializer = new SerializerBuilder()
            .WithEventEmitter(emitter => new ForceQuoteEventEmitter(emitter))
            .Build();
        return serializer.Serialize(section);
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
                currentType = typeof(ConfigSection);
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