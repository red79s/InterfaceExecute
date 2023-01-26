using System;

namespace Eloe.InterfaceSerializer
{
    public class SerializedExecutionContext
    {
        public string InterfaceFullName { get; set; }
        public string MethodName { get; set; }
        public string UniqueMethodName { get; set; }
        public string Payload { get; set; }
        public string ReturnValue { get; set; }
        public bool HaveReturnValue { get; set; }
        public Type ReturnType { get; set; }
        public string ClientId { get; set; }
        public Exception Exception { get; set; }
    }
}