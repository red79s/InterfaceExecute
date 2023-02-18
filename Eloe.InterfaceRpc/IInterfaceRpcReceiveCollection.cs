using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;

namespace Eloe.InterfaceRpc
{
    public interface IInterfaceRpcReceiveCollection
    {
        FunctionReturnDataPacket HandleFunctionCall(FunctionDataPacket package, IAuthorizeHandler authorizeHandler = null);
        void ImplementInterface<U>(U instance) where U : class;
    }
}