﻿using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace Eloe.WebSocketInterfaceRpc
{
    public class WebSocketInterfaceRpcServer : InterfaceRpcServerBase, IServerInfo
    {
        private WatsonWsServer _server;
        private readonly ILogger _logger;

        public List<ClientInfo> GetConnectedClients()
        {
            if (_server != null)
            {
                return _server.ListClients().Select(c => new ClientInfo { ClientId = c.Guid.ToString() }).ToList();
            }
            return new List<ClientInfo>();
        }

        public event EventHandler<ClientInfo> OnClientConnected;
        public event EventHandler<ClientInfo> OnClientDisconnected;

        public WebSocketInterfaceRpcServer(string hostname, int port, ILogger logger)
            : base(logger, true)
        {
            _logger = logger;

            _server = new WatsonWsServer(hostname, port);
            _server.MessageReceived += _server_MessageReceived;
            _server.ClientConnected += _server_ClientConnected;
            _server.ClientDisconnected += _server_ClientDisconnected;
        }

        private void _server_ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            _logger.Debug($"Client disconnected, Ip: {e.Client.Ip}, Port: {e.Client.Port}");

            OnDisconnected();
            OnClientDisconnected?.Invoke(this, new ClientInfo { ClientId = e.Client.Guid.ToString() });
        }

        private void _server_ClientConnected(object sender, ConnectionEventArgs e)
        {
            _logger.Debug($"Client connected, Ip: {e.Client.Ip}, Port: {e.Client.Port}");

            OnConnected();
            OnClientConnected?.Invoke(this, new ClientInfo { ClientId = e.Client.Guid.ToString() });
        }

        public void Start()
        {
            _server.Start();
        }

        private void _server_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var msg = Encoding.UTF8.GetString(e.Data.ToArray());
            _logger.Debug($"Message received: {msg}");

            OnMessageReceived(new MessageReceivedServerArgs { ClientId = e.Client.Guid.ToString(), Data = e.Data.ToArray() });
        }

        protected override Task<bool> SendAsync(SendDataInfo e)
        {
            if (string.IsNullOrEmpty(e.ClientId))
                throw new ArgumentOutOfRangeException(nameof(e.ClientId));

            _logger.Debug("Sending data: " + Encoding.UTF8.GetString(e.Data));

            var guid = Guid.Parse(e.ClientId);
            return _server.SendAsync(guid, e.Data);
        }
    }
}
