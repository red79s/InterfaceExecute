using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcReceiveCollection : IInterfaceRpcReceiveCollection
    {
        private readonly ILogger _logger;
        private Dictionary<string, IInterfaceExecute> _implementedInterfaces = new Dictionary<string, IInterfaceExecute>();

        public InterfaceRpcReceiveCollection(
            ILogger logger)
        {
            _logger = logger;
        }

        public FunctionReturnDataPacket HandleFunctionCall(FunctionDataPacket package, IAuthorizeHandler authorizeHandler)
        {
            var impl = _implementedInterfaces.FirstOrDefault(x => x.Key == package.ClassName);
            if (impl.Key == null)
            {
                _logger.Warn($"Received call for class: {package.ClassName} that have no registered implementations");
                return new FunctionReturnDataPacket
                {
                    Id = package.Id,
                    Exception = new Exception($"Received call for class: {package.ClassName} that have no registered implementations")
                };
            }

            try
            {
                var res = impl.Value.Execute(package.FunctionName, package.FunctionParameters, package.JwtToken, authorizeHandler);

                return new FunctionReturnDataPacket
                {
                    Id = package.Id,
                    ReturnValue = res
                };
            }
            catch (Exception ex)
            {
                return new FunctionReturnDataPacket
                {
                    Id = package.Id,
                    Exception = ex
                };
            }
        }

        public void ImplementInterface<U>(U instance) where U : class
        {
            var iExec = new InterfaceExecute<U>(instance);
            if (_implementedInterfaces.ContainsKey(iExec.InterfaceFullName))
            {
                throw new Exception($"Interface: {iExec.InterfaceFullName} can only be implemented once");
            }

            _implementedInterfaces.Add(iExec.InterfaceFullName, iExec);
        }
    }
}
