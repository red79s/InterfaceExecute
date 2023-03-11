using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using System;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcClient
    {
        private readonly IInterfaceComunicationChannelClient _comunicationChannel;
        private readonly IDataPacketFactory _dataPacketFactory;
        private readonly ILogger _logger;
        private readonly bool _logComunication;
        private object _sendLock = new object();

        private InterfaceRpcReceiveCollectionSendReceive _receiveCollection;
        private InterfaceRpcSendCollection _sendCollection;

        public InterfaceRpcClient(IInterfaceComunicationChannelClient comunicationChannel, ILogger logger, bool logComunication = false)
        {
            _dataPacketFactory = new DataPacketFactory(new DataPacketEncoding(), new FunctionDataPacketEncoding(), new FunctionReturnDataPacketEncoding());

            _receiveCollection = new InterfaceRpcReceiveCollectionSendReceive(_dataPacketFactory, logger);
            _receiveCollection.OnSendData += HandleOnSendData;
            _sendCollection = new InterfaceRpcSendCollection(_dataPacketFactory, logger);
            _sendCollection.OnSendData += HandleOnSendData;

            _comunicationChannel = comunicationChannel;
            if (_comunicationChannel == null && this is IInterfaceComunicationChannelClient)
            {
                _comunicationChannel = (IInterfaceComunicationChannelClient)this;
            }

            _logger = logger;
            _logComunication = logComunication;
            _comunicationChannel.OnMessageReceived += _comunicationChannel_OnMessageReceived;
        }

        public void OnDisconnect()
        {
            _sendCollection.AbortAllExecution();
        }

        private void _comunicationChannel_OnMessageReceived(object sender, MessageReceivedClientArgs e)
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
                _comunicationChannel.SendAsync(e.Data);
            }
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
