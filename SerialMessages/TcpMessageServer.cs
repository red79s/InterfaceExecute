using Eloe.InterfaceSerializer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SerialMessages
{
    public class TcpMessageServer<T> where T : IDataMessage, new()
    {
        private readonly int _port;
        private TcpListener _tcpListener;
        private Thread _listenThread;
        private List<TcpMessageServerConnection<T>> _connections = new List<TcpMessageServerConnection<T>>();
        private Dictionary<string, IInterfaceExecute> _implementedInterfaces = new Dictionary<string, IInterfaceExecute>();

        public TcpMessageServer(int port)
        {
            _port = port;
        }

        public void ImplementInterface<U>(U instance) where U: class
        {
            var iExec = new InterfaceExecute<U>(instance);
            if (_implementedInterfaces.ContainsKey(iExec.InterfaceName))
            {
                throw new Exception($"Interface: {iExec.InterfaceName} can only be implemented once");
            }
            iExec.OnEventInvoked += EventInvoked;
            _implementedInterfaces.Add(iExec.InterfaceName, iExec);
        }

        private void EventInvoked(object sender, EventInvokeData eventData)
        {
            foreach(var connection in _connections)
            {
                connection.InvokeEvent(eventData);
            }
        }
        public void Start()
        {
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _listenThread = new Thread(ListenForConnections);
            _listenThread.IsBackground = true;
            _listenThread.Start();
        }

        private void ListenForConnections()
        {
            try
            {
                _tcpListener.Start();
                while (true)
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();
                    _connections.Add(new TcpMessageServerConnection<T>(tcpClient, _implementedInterfaces));
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }
    }
}
