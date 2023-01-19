using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace WebSocketClient
{
    internal class WsClient : InterfaceRpcClient, IInterfaceComunicationChannelClient
    {
        private WatsonWsClient _client;

        public event EventHandler<MessageReceivedClientArgs> OnMessageReceived;

        public WsClient(ILogger logger)
            : base(null, logger)
        {
        }

        public void Connect(string hostname, int port)
        {
            _client = new WatsonWsClient(hostname, port, false);
            _client.ServerConnected += (s, e) => Console.WriteLine("Connected to server");
            _client.ServerDisconnected += (s, e) => Console.WriteLine("Disconnected from server");

            _client.MessageReceived += _client_MessageReceived;

            _client.Start();
        }

        private void _client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessageReceived?.Invoke(this, new MessageReceivedClientArgs { Data = e.Data.ToArray() });
            Console.WriteLine("Received data: " + Encoding.UTF8.GetString(e.Data.ToArray()));
        }

        public Task SendAsync(byte[] data)
        {
            return _client.SendAsync(data);
        }
    }
}
