namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IDataPacketEncoding
    {
        DataPacketInfo Decode(byte[] buffer);
        byte[] Encode(DataPacketType packetType, byte[] data);
    }
}