using System;
using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public interface IInterfaceComunicationChannelClient : IInterfaceComunicationChannel
    {
        event EventHandler<MessageReceivedClientArgs> OnMessageReceived;
        Task<bool> SendAsync(byte[] data);
    }
}
