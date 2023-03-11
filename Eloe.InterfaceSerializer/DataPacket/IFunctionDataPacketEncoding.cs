namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IFunctionDataPacketEncoding
    {
        FunctionDataPacket Decode(byte[] data);
        byte[] Encode(FunctionDataPacket dataPacket);
    }
}