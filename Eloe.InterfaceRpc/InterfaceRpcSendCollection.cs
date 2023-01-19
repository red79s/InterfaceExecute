using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcSendCollection
    {
        public EventHandler<SendDataInfo> OnSendData;

        private readonly IDataPacketFactory _dataPacketFactory;
        private readonly ILogger _logger;
        private Dictionary<string, IInterfaceExecute> _proxyInterfaces = new Dictionary<string, IInterfaceExecute>();
        private Dictionary<int, TaskCompletionSource<FunctionReturnDataPacketInfo>> _waitingForReturnValues =
            new Dictionary<int, TaskCompletionSource<FunctionReturnDataPacketInfo>>();
        private object _lockObj = new object();
        private int _functionReturnWaitTime = 30000;

        public InterfaceRpcSendCollection(
            IDataPacketFactory dataPacketFactory,
            ILogger logger)
        {
            _dataPacketFactory = dataPacketFactory;
            _logger = logger;
        }

        public void MessageReceived(string clientId, DataPacketInfo dataPacket)
        {
            switch (dataPacket.PackageType)
            {
                case DataPacketType.FunctionReturn:
                    HandleFunctionReturnCall(clientId, _dataPacketFactory.DecodeFunctionReturnCall(dataPacket.Data));
                    break;
                default:
                    throw new Exception($"Invalid package type: {dataPacket.PackageType}");
            }
        }

        private void HandleFunctionReturnCall(string clientId, FunctionReturnDataPacketInfo packet)
        {
            TaskCompletionSource<FunctionReturnDataPacketInfo> taskCompletionSource = null;

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

        public U AddProxyCallbackInterface<U>() where U : class
        {
            var iExec = new InterfaceExecute<U>();
            if (_proxyInterfaces.ContainsKey(iExec.InterfaceFullName))
            {
                throw new Exception($"Interface: {iExec.InterfaceFullName} can only be added once");
            }

            iExec.OnExecute += HandleProxyCallbackOnExecute;
            _proxyInterfaces.Add(iExec.InterfaceFullName, iExec);
            return iExec.GetInterface();
        }

        private volatile int _nextFunctionId = 1;
        private void HandleProxyCallbackOnExecute(object sender, SerializedExecutionContext context)
        {
            var id = _nextFunctionId++;
            var package = _dataPacketFactory.CreateFunctionCall(id, context.InterfaceFullName, context.MethodName, context.Payload);
            OnSendData?.Invoke(this, new SendDataInfo { ClientId = null, Data = package });
            var t = new TaskCompletionSource<FunctionReturnDataPacketInfo>();

            lock (_lockObj)
            {
                _waitingForReturnValues.Add(id, t);
            }

            var notTimeout = t.Task.Wait(_functionReturnWaitTime);
            if (!notTimeout)
            {
                throw new Exception("timeout while waiting for function return");
            }

            if (!string.IsNullOrEmpty(t.Task.Result.Exception))
            {
                throw new Exception(t.Task.Result.Exception);
            }

            context.ReturnValue = t.Task.Result.ReturnValue;
        }
    }
}
