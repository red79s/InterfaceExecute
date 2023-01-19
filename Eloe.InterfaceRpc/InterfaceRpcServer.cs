using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using System;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcServer
    {
        private readonly IInterfaceComunicationChannelServer _comunicationChannel;
        private readonly IDataPacketFactory _dataPacketFactory;
        private readonly ILogger _logger;
        private object _sendLock = new object();

        private InterfaceRpcReceiveCollection _receiveCollection;
        private InterfaceRpcSendCollection _sendCollection;

        public InterfaceRpcServer(IInterfaceComunicationChannelServer comunicationChannel,
            ILogger logger)
        {
            _dataPacketFactory = new DataPacketFactory(new DataPacketEncoding(), new FunctionDataPacketEncoding(), new FunctionReturnDataPacketEncoding());

            _receiveCollection = new InterfaceRpcReceiveCollection(_dataPacketFactory, logger);
            _receiveCollection.OnSendData += HandleOnSendData;
            _sendCollection = new InterfaceRpcSendCollection(_dataPacketFactory, logger);
            _sendCollection.OnSendData += HandleOnSendData;

            _comunicationChannel = comunicationChannel;
            _logger = logger;
            _comunicationChannel.OnMessageReceived += _comunicationChannel_OnMessageReceived;
        }

        private void _comunicationChannel_OnMessageReceived(object sender, MessageReceivedServerArgs e)
        {
            var packet = _dataPacketFactory.DecodeDataPacket(e.Data);
            switch (packet.PackageType)
            {
                case DataPacketType.FunctionCall:
                    _receiveCollection.MessageReceived(e.ClientId, packet);
                    break;
                case DataPacketType.FunctionReturn:
                    _sendCollection.MessageReceived(e.ClientId, packet);
                    break;
                default:
                    throw new Exception("Invalid DataPackageType: " + packet.PackageType);
            }
        }

        private void HandleOnSendData(object sender, SendDataInfo e) 
        {
            lock (_sendLock)
            {
                _comunicationChannel.SendAsync(e.ClientId, e.Data);
            }
        }

        public void ImplementInterface<U>(U instance) where U : class
        {
            _receiveCollection.ImplementInterface(instance);
        }

        public U AddClientCallbackInterface<U>() where U : class
        {
            return _sendCollection.AddProxyCallbackInterface<U>();
        }
    }   
}
