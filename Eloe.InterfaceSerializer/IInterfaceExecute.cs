using System;
using System.Collections.Generic;

namespace Eloe.InterfaceSerializer
{
    public interface IInterfaceExecute
    {
        Type InterfaceType { get; }
        MethodInf GetMetodInfo(string method);
        byte[] Execute(string method, List<byte[]> serializeParameters, string jwtToken = null, IAuthorizeHandler authorizeHandler = null);
        string ClientId { get; }
    }
}