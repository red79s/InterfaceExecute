using System.Collections.Generic;

namespace Eloe.InterfaceSerializer
{
    public interface IInterfaceExecute
    {
        bool HaveMethod(string methodPath);
        List<MethodPathInfo> GetMethodPaths();
        MethodInf GetMetodInfo(string method);
        string Execute(string method, string parametersStr);
    }
}