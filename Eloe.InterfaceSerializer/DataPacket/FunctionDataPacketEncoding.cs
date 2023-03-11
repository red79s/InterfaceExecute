namespace Eloe.InterfaceSerializer.DataPacket
{
    public class FunctionDataPacketEncoding : IFunctionDataPacketEncoding
    {
        private readonly IParameterSerializer _parameterSerializer;

        public FunctionDataPacketEncoding(IParameterSerializer parameterSerializer = null)
        {
            _parameterSerializer = parameterSerializer ?? new MsgPackParameterSerializer();
        }

        public byte[] Encode(FunctionDataPacket dataPacket)
        {
            return _parameterSerializer.Serialize("", typeof(FunctionDataPacket), dataPacket);
        }

        public FunctionDataPacket Decode(byte[] data)
        {
            return _parameterSerializer.Deserialize("", typeof(FunctionDataPacket), data) as FunctionDataPacket;
        }
    }
}
