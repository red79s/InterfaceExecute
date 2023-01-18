using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public class PingMessage : Message, IPingMessage
    {
        public PingMessage(List<byte[]> properties) 
            : base(properties)
        {
            Counter = BitConverter.ToInt32(properties[2], 0);
        }

        public PingMessage(string id, int counter)
            : base (MessageType.Ping, id)
        {
            Counter = counter;
        }

        public override List<byte[]> GetProperties()
        {
            var properties = base.GetProperties();
            properties.Add(BitConverter.GetBytes(Counter));
            return properties;
        }
        public int Counter { get; set; }
    }
}
