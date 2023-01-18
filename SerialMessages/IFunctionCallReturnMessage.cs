using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    interface IFunctionCallReturnMessage : IMessage
    {
        string ReturnValue { get; set; }
    }
}
