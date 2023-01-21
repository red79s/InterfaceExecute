using System.Collections.Generic;

namespace Eloe.InterfaceRpc
{
    public interface IServerInfo
    {
        List<string> ConnectedClientIds { get; }
    }
}
