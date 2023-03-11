using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcSendCollection
    {
        public EventHandler<SendDataInfo> OnSendData;

        private readonly IDataPacketFactory _dataPacketFactory;
        private readonly ILogger _logger;
        private List<IInterfaceExecute> _proxyInterfaces = new List<IInterfaceExecute>();
        private Dictionary<int, TaskCompletionSource<FunctionReturnDataPacket>> _waitingForReturnValues =
            new Dictionary<int, TaskCompletionSource<FunctionReturnDataPacket>>();
        private object _lockObj = new object();
        private int _functionReturnWaitTime = 30000;

        public InterfaceRpcSendCollection(
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
                case DataPacketType.FunctionReturn:
                    HandleFunctionReturnCall(clientId, _dataPacketFactory.DecodeFunctionReturnCall(dataPacket.Data));
                    break;
                default:
                    throw new Exception($"Invalid package type: {dataPacket.PackageType}");
            }
        }

        private void HandleFunctionReturnCall(string clientId, FunctionReturnDataPacket packet)
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

        public U AddProxyCallbackInterface<U>(string clientId = null) where U : class
        {
            var iExeExisting = (InterfaceExecute<U>)_proxyInterfaces.FirstOrDefault(x => x.ClientId == clientId && x.InterfaceType == typeof(U));
            if (iExeExisting != null)
                return iExeExisting.GetInterface();

            var iExec = new InterfaceExecute<U>();
            iExec.ClientId = clientId;
            iExec.OnExecute += HandleProxyCallbackOnExecute;
            _proxyInterfaces.Add(iExec);
            return iExec.GetInterface();
        }

        public void RemoveProxyCallbackInterface<U>(string clientId) where U : class
        {
            var iExe = (InterfaceExecute<U>)_proxyInterfaces.FirstOrDefault(x => x.ClientId == clientId && x.InterfaceType == typeof(U));
            if (iExe != null)
            {
                _proxyInterfaces.Remove(iExe);
                iExe.OnExecute -= HandleProxyCallbackOnExecute;
            }
        }

        public void OnConnected()
        {
            _connected = true;
        }

        public void OnDisconnected()
        {
            _connected = false;
            AbortAllExecution();
        }

        public void AbortAllExecution()
        {
            lock (_cacellationLockObj)
            {
                foreach (var cancellationToken in _cancellationTokenSources)
                {
                    cancellationToken.Cancel();
                }
            }
        }

        private volatile bool _connected;
        private object _cacellationLockObj = new object();
        private List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
        private volatile int _nextFunctionId = 1;
        private void HandleProxyCallbackOnExecute(object sender, SerializedExecutionContext context)
        {
            if (!_connected)
            {
                context.Exception = new Exception("not connected");
                return;
            }

            var id = _nextFunctionId++;

            var t = new TaskCompletionSource<FunctionReturnDataPacket>();
            lock (_lockObj)
            {
                _waitingForReturnValues.Add(id, t);
            }

            var package = _dataPacketFactory.CreateFunctionCall(id, context.InterfaceFullName, context.UniqueMethodName, context.MethodParameters);
            OnSendData?.Invoke(this, new SendDataInfo { ClientId = context.ClientId, Data = package });
            
            //allow all functions waiting for return value to be canceled in case of a disconnect.
            var cancellationToken = new CancellationTokenSource();
            lock (_cacellationLockObj)
            {
                _cancellationTokenSources.Add(cancellationToken);
            }

            try
            {
                var timeoutElapsed = !t.Task.Wait(_functionReturnWaitTime, cancellationToken.Token);
                lock (_cacellationLockObj)
                {
                    _cancellationTokenSources.Remove(cancellationToken);
                }
                if (timeoutElapsed)
                {
                    context.Exception = new Exception("timeout while waiting for function return");
                    return;
                }
            }
            catch(Exception ex)
            {
                lock (_cacellationLockObj)
                {
                    _cancellationTokenSources.Remove(cancellationToken);
                }
                context.Exception = new Exception($"Function call was canceled, ex: {ex.Message}");
                return;
            }

            if (t.Task.Result.Exception != null)
            {
                context.Exception = t.Task.Result.Exception;
                return;
            }
            
            context.ReturnValue = t.Task.Result.ReturnValue;
        }
    }
}
