using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public interface IPingMessage : IMessage
    {
        int Counter { get; set; }
    }
}
