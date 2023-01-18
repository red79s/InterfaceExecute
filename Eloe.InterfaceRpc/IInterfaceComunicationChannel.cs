using System;
using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public interface IInterfaceComunicationChannel
    {
        event EventHandler<MessageReceivedArgs> OnMessageReceived;
    }
}
