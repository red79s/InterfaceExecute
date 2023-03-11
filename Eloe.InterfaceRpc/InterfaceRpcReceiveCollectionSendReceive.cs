using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcReceiveCollectionSendReceive
    {
        public EventHandler<SendDataInfo> OnSendData;
        private readonly IDataPacketFactory _dataPacketFactory;
        private readonly ILogger _logger;
        private Dictionary<string, IInterfaceExecute> _implementedInterfaces = new Dictionary<string, IInterfaceExecute>();

        public InterfaceRpcReceiveCollectionSendReceive(
            IDataPacketFactory dataPacketFactory,
            ILogger logger)
        {
            _dataPacketFactory = dataPacketFactory;
            _logger = logger;
        }

        public void MessageReceived(string clientId, DataPacket dataPacket)
        {
            switch (dataPacket.PackageType)
            {
                case DataPacketType.FunctionCall:
                    HandleFunctionCall(clientId, _dataPacketFactory.DecodeFunctionCall(dataPacket.Data));
                    break;
                default:
                    throw new Exception("Invalid DataPackageType: " + dataPacket.PackageType);
            }
        }

        private void HandleFunctionCall(string clientId, FunctionDataPacket package)
        {
            Task.Run(() =>
            {
                byte[] returnPackage = null;
                var impl = _implementedInterfaces.FirstOrDefault(x => x.Key == package.ClassName);
                if (impl.Key == null)
                {
                    _logger.Warn($"Received call for class: {package.ClassName} that have no registered implementations");
                    returnPackage = _dataPacketFactory.CreateFuctionReturnCall(package.Id, null, new Exception($"Received call for class: {package.ClassName} that have no registered implementations"));
                }

                if (returnPackage == null)
                {
                    try
                    {
                        var res = impl.Value.Execute(package.FunctionName, package.FunctionParameters);

                        returnPackage = _dataPacketFactory.CreateFuctionReturnCall(package.Id, res, null);
                    }
                    catch (Exception ex)
                    {
                        returnPackage = _dataPacketFactory.CreateFuctionReturnCall(package.Id, null, ex);
                    }
                }

                OnSendData?.Invoke(this, new SendDataInfo { ClientId = clientId, Data = returnPackage });
            });
        }

        public void ImplementInterface<U>(U instance) where U : class
        {
            var iExec = new InterfaceExecute<U>(instance);
            if (_implementedInterfaces.ContainsKey(iExec.InterfaceFullName))
            {
                throw new Exception($"Interface: {iExec.InterfaceFullName} can only be implemented once");
            }

            _implementedInterfaces.Add(iExec.InterfaceFullName, iExec);
        }
    }
}
