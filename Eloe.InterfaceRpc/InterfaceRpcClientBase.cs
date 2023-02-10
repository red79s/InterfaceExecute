using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using System;
using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcClientBase
    {
        private readonly IDataPacketFactory _dataPacketFactory;
        private readonly ILogger _logger;
        private readonly bool _logComunication;
        private object _sendLock = new object();

        private InterfaceRpcReceiveCollectionSendReceive _receiveCollection;
        private InterfaceRpcSendCollection _sendCollection;

        public InterfaceRpcClientBase(ILogger logger, bool logComunication = false)
        {
            _dataPacketFactory = new DataPacketFactory(new DataPacketEncoding(), new FunctionDataPacketEncoding(), new FunctionReturnDataPacketEncoding());

            _receiveCollection = new InterfaceRpcReceiveCollectionSendReceive(_dataPacketFactory, logger);
            _receiveCollection.OnSendData += HandleOnSendData;
            _sendCollection = new InterfaceRpcSendCollection(_dataPacketFactory, logger);
            _sendCollection.OnSendData += HandleOnSendData;

            _logger = logger;
            _logComunication = logComunication;
        }

        public void OnDisconnect()
        {
            _sendCollection.OnDisconnected();
        }

        public void OnConnected()
        {
            _sendCollection.OnConnected();
        }

        protected void OnMessageReceived(MessageReceivedClientArgs e)
        {
            var packet = _dataPacketFactory.DecodeDataPacket(e.Data);

            if (_logComunication)
            {
                LogReceivedPacket(packet);
            }

            switch (packet.PackageType)
            {
                case DataPacketType.FunctionCall:
                    _receiveCollection.MessageReceived(null, packet);
                    break;
                case DataPacketType.FunctionReturn:
                    _sendCollection.MessageReceived(null, packet);
                    break;
                default:
                    throw new Exception("Invalid DataPackageType: " + packet.PackageType);
            }
        }

        private void LogReceivedPacket(DataPacket dataPacket)
        {
            if (dataPacket == null)
            {
                _logger.Error("Received empty data packet");
                return;
            }

            switch (dataPacket.PackageType)
            {
                case DataPacketType.FunctionCall:
                    var functionCall = _dataPacketFactory.DecodeFunctionCall(dataPacket.Data);
                    _logger.Debug($"Type: {dataPacket.PackageType} Id: {functionCall.Id}, Class: {functionCall.ClassName}, Function: {functionCall.FunctionName}");
                    break;
                case DataPacketType.FunctionReturn:
                    var functionReturn = _dataPacketFactory.DecodeFunctionReturnCall(dataPacket.Data);
                    _logger.Debug($"Type: {dataPacket.PackageType} Id: {functionReturn.Id}, ReturnValue: {functionReturn.ReturnValue}, Exception: {functionReturn.Exception}");
                    break;
                default:
                    _logger.Error($"Invalid data package: {dataPacket.PackageType}");
                    break;
            }
        }

        private void HandleOnSendData(object sender, SendDataInfo e)
        {
            lock (_sendLock)
            {
                SendAsync(e);
            }
        }

        protected virtual Task<bool> SendAsync(SendDataInfo e)
        {
            throw new NotImplementedException();
        }

        public void ImplementInterface<U>(U instance) where U : class
        {
            _receiveCollection.ImplementInterface(instance);
        }

        public U AddServerInterface<U>() where U : class
        {
            return _sendCollection.AddProxyCallbackInterface<U>();
        }
    }
}
