using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public interface IMessage
    {
        MessageType MessageType { get; set; }
        string Id { get; set; }
        List<byte[]> GetProperties();
        string GetString();
    }
}
