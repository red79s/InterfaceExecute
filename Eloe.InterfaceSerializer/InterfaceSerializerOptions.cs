namespace Eloe.InterfaceSerializer
{
    public class InterfaceSerializerOptions : IInterfaceSerializerOptions
    {
        public IParameterSerializer ParameterSerializer { get; set; }
        public InterfaceSerializerOptions()
        {
            ParameterSerializer = new MsgPackParameterSerializer();
        }
    }
}
