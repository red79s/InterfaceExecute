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

        public byte[] CreateFunctionCall(int id, string className, string functionName, string functionParameters)
        {
            var payload = _functionDataPacketEncoding.Encode(id, className, functionName, functionParameters);
            return _dataPacketEncoding.Encode(DataPacketType.FunctionCall, payload);
        }

        public byte[] CreateFuctionReturnCall(int id, string returnValue, string exception)
        {
            var payload = _functionReturnDataPacketEncoding.Encode(id, returnValue, exception);
            return _dataPacketEncoding.Encode(DataPacketType.FunctionReturn, payload);
        }

        public DataPacketInfo DecodeDataPacket(byte[] data)
        {
            return _dataPacketEncoding.Decode(data);
        }

        public FunctionDataPacketInfo DecodeFunctionCall(byte[] data)
        {
            return _functionDataPacketEncoding.Decode(data);
        }

        public FunctionReturnDataPacketInfo DecodeFunctionReturnCall(byte[] data)
        {
            return _functionReturnDataPacketEncoding.Decode(data);
        }
    }
}
