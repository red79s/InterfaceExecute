namespace Eloe.InterfaceSerializer
{
    public interface IInterfaceSerializerOptions
    {
        IParameterSerializer ParameterSerializer { get; set; }
    }
}