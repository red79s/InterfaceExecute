using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using System.Text;
using WatsonWebsocket;

namespace WebSocketServer
{
    internal class WsServer : InterfaceRpcServer, IInterfaceComunicationChannelServer
    {
        private WatsonWsServer _server;

        public event EventHandler<MessageReceivedServerArgs>? OnMessageReceived;

        public WsServer(string hostname, int port, ILogger logger)
            : base(null, logger, true)
        {
            _server = new WatsonWsServer(hostname, port);
            _server.MessageReceived += _server_MessageReceived;
            _server.ClientConnected += (s, e) => Console.WriteLine("Client connected");
            _server.ClientDisconnected += (s, e) => Console.WriteLine("Client disconnected");
        }

        public void Start()
        {
            _server.Start();
        }

        private void _server_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            OnMessageReceived?.Invoke(this, new MessageReceivedServerArgs { ClientId = e.Client.Guid.ToString(), Data = e.Data.ToArray() });
        }

        public Task<bool> SendAsync(string client, byte[] data)
        {
            if (string.IsNullOrEmpty(client))
                throw new ArgumentOutOfRangeException(nameof(client));

            var guid = Guid.Parse(client);
            return _server.SendAsync(guid, data);
        }
    }
}
