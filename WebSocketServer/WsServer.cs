using System.Text;
using WatsonWebsocket;

namespace WebSocketServer
{
    internal class WsServer
    {
        WatsonWsServer _server;
        public WsServer(string hostname, int port)
        {
            _server = new WatsonWsServer(hostname, port);
            _server.MessageReceived += _server_MessageReceived;
            _server.ClientConnected += (s, e) => Console.WriteLine("Client connected");
            _server.ClientDisconnected += (s, e) => Console.WriteLine("Client disconnected");
        }

        private void _server_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            var data = Encoding.UTF8.GetString(e.Data);
            Console.WriteLine("Recived message: " + data);
            _server.SendAsync(e.Client.Guid, Encoding.UTF8.GetBytes("Response: " + data));
        }

        public void Start()
        {
            _server.Start();
        }
    }
}
