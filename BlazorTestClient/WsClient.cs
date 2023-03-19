using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using System.Net.WebSockets;
using System.Text;
using WatsonWebsocket;

namespace BlazorTestClient
{
    public class WsClient : InterfaceRpcWasmClientBase
    {
        private ClientWebSocket _client;
        private CancellationToken _cancellationToken;
        public WsClient(Eloe.InterfaceSerializer.ILogger logger)
            : base(logger, true)
        {
            _client = new ClientWebSocket();
            _cancellationToken = new CancellationToken();
        }

        public async Task Connect(string hostname, int port)
        {
            await _client.ConnectAsync(new Uri($"ws://{hostname}:{port}"), _cancellationToken);
            OnConnected();
            _ = ReceiveLoop();
        }

        async Task ReceiveLoop()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            var messageBuffer = new List<byte>();
            while (!_cancellationToken.IsCancellationRequested)
            {
                // Note that the received block might only be part of a larger message. If this applies in your scenario,
                // check the received.EndOfMessage and consider buffering the blocks until that property is true.
                // Or use a higher-level library such as SignalR.
                var received = await _client.ReceiveAsync(buffer, _cancellationToken);
                messageBuffer.AddRange(buffer.Take(received.Count));
                if (received.EndOfMessage)
                {
                    var msg = Encoding.UTF8.GetString(messageBuffer.ToArray());
                    Console.WriteLine("Receive loop: " + msg);
                    OnMessageReceived(new MessageReceivedClientArgs { Data = messageBuffer.ToArray() });
                    messageBuffer.Clear();
                }
            }
        }

        public Task<bool> SendDataAsync(byte[] data)
        {
            return SendAsync(new SendDataInfo { Data = data });
        }
        protected override async Task<bool> SendAsync(SendDataInfo e)
        {
            await _client.SendAsync(e.Data, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage, _cancellationToken);
            return true;
        }
    }
}
