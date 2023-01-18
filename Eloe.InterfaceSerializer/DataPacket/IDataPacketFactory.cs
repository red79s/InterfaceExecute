namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IDataPacketFactory
    {
        byte[] CreateFuctionReturnCall(int id, string returnValue, string exception);
        byte[] CreateFunctionCall(int id, string className, string functionName, string functionParameters);
        DataPacketInfo DecodeDataPacket(byte[] data);
        FunctionDataPacketInfo DecodeFunctionCall(byte[] data);
        FunctionReturnDataPacketInfo DecodeFunctionReturnCall(byte[] data);
    }
}