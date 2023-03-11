using System;

namespace Eloe.InterfaceSerializer
{
    public interface IParameterSerializer
    {
        byte[] Serialize(string name, Type type, object value);
        object Deserialize(string name, Type type, byte[] serializedValue);
    }
}
