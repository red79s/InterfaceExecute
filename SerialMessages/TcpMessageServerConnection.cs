using Eloe.InterfaceSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SerialMessages
{
    public class TcpMessageServerConnection<T> where T: IDataMessage, new()
    {
        private readonly IDataMessage _dataMessageManager;
        private readonly TcpClient _tcpClient;
        private readonly Dictionary<string, IInterfaceExecute> _implementedInterfaces;
        private Thread _readThread;
        private object _lockObj = new object();
        private long _id = 0;

        public TcpMessageServerConnection(TcpClient tcpClient, Dictionary<string, IInterfaceExecute> implementedInterfaces)
        {
            _dataMessageManager = new T();
            _dataMessageManager.OnMessageReceived += DataMessageManager_OnMessageReceived;
            _tcpClient = tcpClient;
            _implementedInterfaces = implementedInterfaces;
            _readThread = new Thread(ReadData);
            _readThread.IsBackground = true;
            _readThread.Start();
        }

        private string GetNextId()
        {
            return $"{_id++}";
        }

        public void InvokeEvent(EventInvokeData data)
        {
            var msg = new EventCallMessage(GetNextId(), data.InterfaceName, data.EventName, data.Parameters);
            var buffer = _dataMessageManager.EncodeMessage(msg);
            Send(buffer);
        }

        private void DataMessageManager_OnMessageReceived(object sender, IMessage msg)
        {
            Console.WriteLine(msg.GetString());
            if (msg.MessageType == MessageType.FunctionCall)
            {
                var funcCallMsg = msg as IFunctionCallMessage;
                if (_implementedInterfaces.ContainsKey(funcCallMsg.InterfaceName))
                {
                    var metodInfo = _implementedInterfaces[funcCallMsg.InterfaceName].GetMetodInfo(funcCallMsg.FunctionName);
                    if (metodInfo == null)
                    {

                    }
                    else
                    {
                        var returnStr = _implementedInterfaces[funcCallMsg.InterfaceName].Execute(funcCallMsg.FunctionName, funcCallMsg.Parameters);
                        if (metodInfo.ReturnType != null && metodInfo.ReturnType != typeof(void))
                        {
                            var data = _dataMessageManager.EncodeMessage(new FunctionCallReturnMessage(funcCallMsg.Id, returnStr));
                            Send(data);
                        }
                    }
                }
            }
        }

        private void Send(byte[] data)
        {
            lock (_lockObj)
            {
                _tcpClient.GetStream().Write(data, 0, data.Length);
            }
        }

        private void ReadData()
        {
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = _tcpClient.GetStream().Read(buffer, 0, 1024);
                    if (bytesRead < 1)
                    {
                        //client disconnected
                    }
                    else
                    {
                        _dataMessageManager.DataReceived(buffer.Take(bytesRead).ToArray());
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
