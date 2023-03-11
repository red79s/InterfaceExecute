using MessagePack;
using System;

namespace Eloe.InterfaceSerializer
{
    public class MsgPackParameterSerializer : IParameterSerializer
    {
        public object Deserialize(string name, Type type, byte[] serializedValue)
        {
            return MessagePackSerializer.Deserialize(type, serializedValue, MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }

        public byte[] Serialize(string name, Type type, object value)
        {
            return MessagePackSerializer.Serialize(type, value, MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }
    }
}
