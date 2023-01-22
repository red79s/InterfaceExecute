namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IDataPacketFactory
    {
        byte[] CreateFuctionReturnCall(int id, string returnValue, string exception);
        byte[] CreateFunctionCall(int id, string className, string functionName, string functionParameters);
        DataPacket DecodeDataPacket(byte[] data);
        FunctionDataPacket DecodeFunctionCall(byte[] data);
        FunctionReturnDataPacket DecodeFunctionReturnCall(byte[] data);
    }
}