namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IFunctionReturnDataPacketEncoding
    {
        FunctionReturnDataPacketInfo Decode(byte[] data);
        byte[] Encode(int id, string returnValue, string exception);
    }
}