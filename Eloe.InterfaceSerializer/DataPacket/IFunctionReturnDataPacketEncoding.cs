namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IFunctionReturnDataPacketEncoding
    {
        FunctionReturnDataPacket Decode(byte[] data);
        byte[] Encode(int id, string returnValue, string exception);
    }
}