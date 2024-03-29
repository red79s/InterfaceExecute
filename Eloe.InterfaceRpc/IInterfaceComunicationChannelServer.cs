﻿using System;
using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public interface IInterfaceComunicationChannelServer : IInterfaceComunicationChannel
    {
        event EventHandler<MessageReceivedServerArgs> OnMessageReceived;
        Task<bool> SendAsync(string client, byte[] data);
    }
}
