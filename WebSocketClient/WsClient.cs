using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace WebSocketClient
{
    internal class WsClient : InterfaceRpcClientBase
    {
        private WatsonWsClient _client;
        public WsClient(ILogger logger)
            : base(logger, false)
        {
        }

        public bool IsConnected
        {
            get
            {
                if (_client == null)
                {
                    return false;
                }
                return _client.Connected;
            }
        }

        public void Connect(string hostname, int port)
        {
            Connect(hostname, port, 0);
        }

        private void Connect(string hostname, int port, int delayInMs)
        {
            try
            {
                if (delayInMs > 0)
                {
                    Console.WriteLine($"Trying to reconnect: {delayInMs}");
                }

                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }

                _client = new WatsonWsClient(hostname, port, false);
                _client.ServerConnected += (s, e) =>
                {
                    OnConnected();
                    Console.WriteLine("Connected to server");
                };

                _client.ServerDisconnected += (s, e) =>
                {
                    Console.WriteLine("Disconnected from server");
                    OnDisconnect();
                    Connect(hostname, port, 100);
                };

                _client.MessageReceived += _client_MessageReceived;

                Thread.Sleep(delayInMs);
                if (!_client.StartWithTimeout(10))
                {
                    Console.WriteLine($"Failed to connect");
                    Connect(hostname, port, IncrementDelay(delayInMs));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect: {ex.Message}");
                Connect(hostname, port, IncrementDelay(delayInMs));
            }
        }
        
        private int IncrementDelay(int delayInMs)
        {
            var newDelayInMs = delayInMs * 2;
            if (newDelayInMs > 5000)
            {
                newDelayInMs = 5000;
            }
            return newDelayInMs;
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
