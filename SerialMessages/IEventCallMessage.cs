using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public interface IEventCallMessage : IMessage
    {
        string InterfaceName { get; set; }
        string EventName { get; set; }
        string Parameters { get; set; }
    }
}
