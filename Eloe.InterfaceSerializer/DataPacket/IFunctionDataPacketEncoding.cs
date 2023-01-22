namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IFunctionDataPacketEncoding
    {
        FunctionDataPacket Decode(byte[] data);
        byte[] Encode(int id, string className, string functionName, string functionParameters);
    }
}