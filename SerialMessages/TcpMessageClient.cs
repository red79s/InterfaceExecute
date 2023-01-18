using Eloe.InterfaceSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialMessages
{
    public class TcpMessageClient<T> where T : IDataMessage, new()
    {
        private readonly T _dataMessageManager;
        private TcpClient _tcpClient;
        private Thread _readThread;
        private List<IInterfaceExecute> _executers = new List<IInterfaceExecute>();
        private object _lockObjc = new object();
        private long _id = 1;
        private Dictionary<string, FunctionReturnTaskCompletion<string>> _returnValues = new Dictionary<string, FunctionReturnTaskCompletion<string>>();

        public TcpMessageClient()
        {
            _tcpClient = new TcpClient();
            _dataMessageManager = new T();
            _dataMessageManager.OnMessageReceived += DataMessageManager_OnMessageReceived;
            _readThread = new Thread(ReadData);
            _readThread.IsBackground = true;
            _readThread.Start();
        }

        private string GetNextId()
        {
            lock (_lockObjc)
            {
                return $"{_id++}";
            }
        }
        public Y ImplementInterface<Y>() where Y: class
        {
            var iExec = new InterfaceExecute<Y>();
            iExec.OnExecute += (s, c) =>
            {
                string id = GetNextId();
                var funcCallMsg = new FunctionCallMessage(id, iExec.InterfaceName, c.MethodName, c.Payload);
                var data = _dataMessageManager.EncodeMessage(funcCallMsg);
                Send(data);
                if (c.HaveReturnValue)
                {
                    var t = new FunctionReturnTaskCompletion<string>();
                    t.Id = id;
                    _returnValues.Add(id, t);
                    c.ReturnValue = t.Task.Result;
                }
            };
            return iExec.GetInterface();
        }
        private void DataMessageManager_OnMessageReceived(object sender, IMessage msg)
        {
            Console.WriteLine($"message recived {msg.GetString()}");
            if (msg.MessageType == MessageType.FunctionReturn)
            {
                var funcReturnMessage = (IFunctionCallReturnMessage)msg;
                if (_returnValues.ContainsKey(funcReturnMessage.Id))
                {
                    _returnValues[funcReturnMessage.Id].SetResult(funcReturnMessage.ReturnValue);
                    _returnValues.Remove(funcReturnMessage.Id);
                }
            }
        }

        public void Connect(string hostname, int port)
        {
            _tcpClient.Connect(hostname, port);
        }

        private void Send(byte[] data)
        {
            lock (_lockObjc)
            {
                _tcpClient.GetStream().Write(data, 0, data.Length);
            }
        }

        private void ReadData()
        {
            var buffer = new byte[1024];
            while(true)
            {
                try
                {
                    while(!_tcpClient.Connected)
                    {
                        Thread.Sleep(100);
                    }

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
                catch(ThreadAbortException)
                {
                    return;
                }
            }
        }
    }
}
