using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public interface IFunctionCallMessage : IMessage
    {
        string InterfaceName { get; set; }
        string FunctionName { get; set; }
        string Parameters { get; set; }
    }
}
