using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public class Message : IMessage
    {
        public Message(List<byte[]> properties)
        {
            MessageType = (MessageType)BitConverter.ToInt32(properties[0], 0);
            Id = Encoding.Unicode.GetString(properties[1]);
        }

        public Message(MessageType messageType, string id)
        {
            MessageType = messageType;
            Id = id;
        }

        public virtual List<byte[]> GetProperties()
        {
            return new List<byte[]>
            {
                BitConverter.GetBytes((int)MessageType),
                Encoding.Unicode.GetBytes(Id)
            };
        }

        public virtual string GetString()
        {
            return $"Type: {MessageType}, Id: {Id}";
        }

        public MessageType MessageType { get; set; }
        public string Id { get; set; }
    }
}
