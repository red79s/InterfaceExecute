namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IDataPacketEncoding
    {
        DataPacket Decode(byte[] buffer);
        byte[] Encode(DataPacketType packetType, byte[] data);
    }
}