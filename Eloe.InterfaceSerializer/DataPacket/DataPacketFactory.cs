using Dynamitey.DynamicObjects;
using System;
using System.Collections.Generic;

namespace Eloe.InterfaceSerializer.DataPacket
{
    public class DataPacketFactory : IDataPacketFactory
    {
        private readonly IDataPacketEncoding _dataPacketEncoding;
        private readonly IFunctionDataPacketEncoding _functionDataPacketEncoding;
        private readonly IFunctionReturnDataPacketEncoding _functionReturnDataPacketEncoding;

        public DataPacketFactory(IDataPacketEncoding dataPacketEncoding,
            IFunctionDataPacketEncoding functionDataPacketEncoding,
            IFunctionReturnDataPacketEncoding functionReturnDataPacketEncoding)
        {
            _dataPacketEncoding = dataPacketEncoding;
            _functionDataPacketEncoding = functionDataPacketEncoding;
            _functionReturnDataPacketEncoding = functionReturnDataPacketEncoding;
        }

        public byte[] CreateFunctionCall(int id, string className, string functionName, List<byte[]> functionParameters)
        {
            var dataPacket = new FunctionDataPacket
            {
                Id = id,
                ClassName = className,
                FunctionName = functionName,
                FunctionParameters = functionParameters
            };
            var payload = _functionDataPacketEncoding.Encode(dataPacket);
            return _dataPacketEncoding.Encode(DataPacketType.FunctionCall, payload);
        }

        public byte[] CreateFuctionReturnCall(int id, byte[] returnValue, Exception exception)
        {
            var dataPacket = new FunctionReturnDataPacket { Id = id, ReturnValue = returnValue, Exception = exception };
            var payload = _functionReturnDataPacketEncoding.Encode(dataPacket);
            return _dataPacketEncoding.Encode(DataPacketType.FunctionReturn, payload);
        }

        public DataPacket DecodeDataPacket(byte[] data)
        {
            return _dataPacketEncoding.Decode(data);
        }

        public FunctionDataPacket DecodeFunctionCall(byte[] data)
        {
            return _functionDataPacketEncoding.Decode(data);
        }

        public FunctionReturnDataPacket DecodeFunctionReturnCall(byte[] data)
        {
            return _functionReturnDataPacketEncoding.Decode(data);
        }
    }
}
