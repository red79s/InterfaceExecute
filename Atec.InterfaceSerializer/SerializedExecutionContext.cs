using System;

namespace Atec.InterfaceSerializer
{
    public class SerializedExecutionContext
    {
        public string InterfaceFullName { get; set; }
        public string MethodName { get; set; }
        public string ExecutePath { get; set; }
        public string Payload { get; set; }
        public string ReturnValue { get; set; }
        public bool HaveReturnValue { get; set; }
        public Type ReturnType { get; set; }
    }
}