using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using System;
using System.Linq;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace WebSocketClient
{
    internal class WsClient : InterfaceRpcClientBase
    {
        private WatsonWsClient _client;

        public WsClient(ILogger logger)
            : base(logger, true)
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
            OnMessageReceived(new MessageReceivedClientArgs { Data = e.Data.ToArray() });
        }

        protected override Task<bool> SendAsync(SendDataInfo e)
        {
            return _client.SendAsync(e.Data);
        }
    }
}
