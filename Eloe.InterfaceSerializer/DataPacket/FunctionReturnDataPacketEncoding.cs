using System;
using System.Text;

namespace Eloe.InterfaceSerializer.DataPacket
{
    public class FunctionReturnDataPacketEncoding : IFunctionReturnDataPacketEncoding
    {
        private readonly IParameterSerializer _parameterSerializer;

        public FunctionReturnDataPacketEncoding(IParameterSerializer parameterSerializer = null)
        {
            _parameterSerializer = parameterSerializer ?? new MsgPackParameterSerializer();
        }

        public byte[] Encode(FunctionReturnDataPacket dataPacket)
        {
            return _parameterSerializer.Serialize("", typeof(FunctionReturnDataPacket), dataPacket);
        }

        public FunctionReturnDataPacket Decode(byte[] data)
        {
            return _parameterSerializer.Deserialize("", typeof(FunctionReturnDataPacket), data) as FunctionReturnDataPacket;
        }
    }
}
