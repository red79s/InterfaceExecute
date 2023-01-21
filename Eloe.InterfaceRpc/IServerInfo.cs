using System;
using System.Collections.Generic;

namespace Eloe.InterfaceRpc
{
    public interface IServerInfo
    {
        event EventHandler<ClientInfo> OnClientConnected;
        event EventHandler<ClientInfo> OnClientDisconnected;
        List<ClientInfo> GetConnectedClients();
    }
}
