using Eloe.InterfaceSerializer.DataPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eloe.InterfaceSerializer
{
    public class InterfaceExecuteCollection
    {
        public EventHandler<SendDataInfo> OnSendData;

        private readonly IDataPacketFactory _dataPacketFactory;
        private readonly ILogger _logger;
        private Dictionary<string, IInterfaceExecute> _implementedInterfaces = new Dictionary<string, IInterfaceExecute>();
        private Dictionary<string, IInterfaceExecute> _clientInterfaces = new Dictionary<string, IInterfaceExecute>();
        private Dictionary<int, TaskCompletionSource<FunctionReturnDataPacket>> _waitingForReturnValues =
            new Dictionary<int, TaskCompletionSource<FunctionReturnDataPacket>>();
        private object _lockObj = new object();
        private int _functionReturnWaitTime = 30000;

        public InterfaceExecuteCollection(
            IDataPacketFactory dataPacketFactory,
            ILogger logger)
        {
            _dataPacketFactory = dataPacketFactory;
            _logger = logger;
        }

        private void MessageReceived(string clientId, byte[] data)
        {
            var packet = _dataPacketFactory.DecodeDataPacket(data);
            switch (packet.PackageType)
            {
                case DataPacketType.FunctionCall:
                    HandleFunctionCall(_dataPacketFactory.DecodeFunctionCall(packet.Data), clientId);
                    break;
                case DataPacketType.FunctionReturn:
                    HandleFunctionReturnCall(_dataPacketFactory.DecodeFunctionReturnCall(packet.Data), clientId);
                    break;
                default:
                    throw new Exception("Invalid DataPackageType: " + packet.PackageType);
            }
        }

        private void HandleFunctionCall(FunctionDataPacket package, string clientId)
        {
            var impl = _implementedInterfaces.FirstOrDefault(x => x.Key == package.ClassName);
            if (impl.Key == null)
            {
                _logger.Warn($"Received call for class: {package.ClassName} that have no registered implementations");
                return;
            }

            var res = impl.Value.Execute(package.FunctionName, package.FunctionParameters);

            var returnPackage = _dataPacketFactory.CreateFuctionReturnCall(package.Id, res, null);
            OnSendData?.Invoke(this, new SendDataInfo { ClientId = clientId, Data = returnPackage });
        }

        private void HandleFunctionReturnCall(FunctionReturnDataPacket packet, string clientId)
        {
            TaskCompletionSource<FunctionReturnDataPacket> taskCompletionSource = null;

            lock (_lockObj)
            {
                if (_waitingForReturnValues.ContainsKey(packet.Id))
                {
                    taskCompletionSource = _waitingForReturnValues[packet.Id];
                    _waitingForReturnValues.Remove(packet.Id);
                }
                else
                {
                    _logger.Warn($"Invalid function return data package with non existent id: {packet.Id}");
                }
            }

            if (taskCompletionSource != null)
            {
                taskCompletionSource.SetResult(packet);
            }
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

        public U AddClientCallbackInterface<U>() where U : class
        {
            var iExec = new InterfaceExecute<U>();
            if (_clientInterfaces.ContainsKey(iExec.InterfaceFullName))
            {
                throw new Exception($"Interface: {iExec.InterfaceFullName} can only be added once");
            }

            iExec.OnExecute += HandleClientCallbackOnExecute;
            _clientInterfaces.Add(iExec.InterfaceFullName, iExec);
            return iExec.GetInterface();
        }

        private volatile int _nextFunctionId = 1;
        private void HandleClientCallbackOnExecute(object sender, SerializedExecutionContext context)
        {
            var id = _nextFunctionId++;
            var package = _dataPacketFactory.CreateFunctionCall(id, context.InterfaceFullName, context.MethodName, context.MethodParameters);
            OnSendData?.Invoke(this, new SendDataInfo { ClientId = null, Data = package });
            var t = new TaskCompletionSource<FunctionReturnDataPacket>();

            lock (_lockObj)
            {
                _waitingForReturnValues.Add(id, t);
            }

            var notTimeout = t.Task.Wait(_functionReturnWaitTime);
            if (!notTimeout)
            {
                throw new Exception("timeout while waiting for function return");
            }

            if (t.Task.Result.Exception != null)
            {
                throw new Exception(t.Task.Result.Exception.ToString());
            }

            context.ReturnValue = t.Task.Result.ReturnValue;
        }
    }
}
