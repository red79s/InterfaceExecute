using System;
using System.Collections.Generic;

namespace Eloe.InterfaceSerializer
{
    public interface IInterfaceExecute
    {
        Type InterfaceType { get; }
        MethodInf GetMetodInfo(string method);
        string Execute(string method, string parametersStr);
        string ClientId { get; }
    }
}