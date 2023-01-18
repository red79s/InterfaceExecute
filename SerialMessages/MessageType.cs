using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public enum MessageType
    {
        Ping,
        FunctionCall,
        FunctionReturn,
        EventCall,
        Authenticate
    }
}
