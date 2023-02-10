using ClientServerComDef;
using Eloe.InterfaceRpc;

namespace WebApiServer
{
    public class InterfaceExecuteFactory
    {
        private readonly IInterfaceRpcReceiveCollection _interfaceRpcReceiveCollection;

        public InterfaceExecuteFactory(IInterfaceRpcReceiveCollection interfaceRpcReceiveCollection)
        {
            _interfaceRpcReceiveCollection = interfaceRpcReceiveCollection;
        }

        public IInterfaceRpcReceiveCollection Get()
        {
            var serverFunctions = new ServerFunctionsImpl();
            _interfaceRpcReceiveCollection.ImplementInterface<IServerFunctions>(serverFunctions);
            return _interfaceRpcReceiveCollection;
        }
    }
}
