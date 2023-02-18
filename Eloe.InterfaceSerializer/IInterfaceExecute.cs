using System;

namespace Eloe.InterfaceSerializer
{
    public interface IInterfaceExecute
    {
        Type InterfaceType { get; }
        MethodInf GetMetodInfo(string method);
        string Execute(string method, string parametersStr, string jwtToken = null, IAuthorizeHandler authorizeHandler = null);
        string ClientId { get; }
    }
}