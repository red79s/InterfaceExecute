using Dynamitey.DynamicObjects;
using System;
using System.Collections.Generic;

namespace Eloe.InterfaceSerializer.DataPacket
{
    public interface IDataPacketFactory
    {
        byte[] CreateFuctionReturnCall(int id, byte[] returnValue, Exception exception);
        byte[] CreateFunctionCall(int id, string className, string functionName, List<byte[]> functionParameters);
        DataPacket DecodeDataPacket(byte[] data);
        FunctionDataPacket DecodeFunctionCall(byte[] data);
        FunctionReturnDataPacket DecodeFunctionReturnCall(byte[] data);
    }
}