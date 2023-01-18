using System.Threading.Tasks;

namespace Eloe.InterfaceRpc
{
    public interface IInterfaceComunicationChannelServer : IInterfaceComunicationChannel
    {
        Task SendAsync(string client, byte[] data);
    }
}
