using System;
using System.Linq;
using System.Text;
using WatsonWebsocket;

namespace WebSocketClient
{
    internal class WsClient
    {
        private WatsonWsClient _client;
        public WsClient(string hostname, int port) 
        {
            _client = new WatsonWsClient(hostname, port, false);
            _client.ServerConnected += (s, e) => Console.WriteLine("Connected to server");
            _client.ServerDisconnected += (s, e) => Console.WriteLine("Disconnected from server");

            _client.MessageReceived += _client_MessageReceived;
        }

        public void Connect()
        {
            _client.Start();
        }

        private void _client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Received data: " + Encoding.UTF8.GetString(e.Data.ToArray()));
        }

        public void SendMessage(string message)
        {
            _client.SendAsync(Encoding.UTF8.GetBytes(message));
        }
    }
}
