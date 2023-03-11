using System;

namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IFunctionReturnDataPacketEncoding
    {
        FunctionReturnDataPacket Decode(byte[] data);
        byte[] Encode(FunctionReturnDataPacket dataPacket);
    }
}