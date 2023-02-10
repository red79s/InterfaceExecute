using Eloe.InterfaceSerializer.DataPacket;

namespace Eloe.InterfaceRpc
{
    public interface IInterfaceRpcReceiveCollection
    {
        FunctionReturnDataPacket HandleFunctionCall(FunctionDataPacket package);
        void ImplementInterface<U>(U instance) where U : class;
    }
}