using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public interface IInterfaceComunicationChannelClient : IInterfaceComunicationChannel
    {
        Task SendAsync(byte[] data);
    }
}
