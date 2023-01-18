using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public class EventCallMessage : Message, IEventCallMessage
    {
        public EventCallMessage(List<byte[]> properties)
            : base(properties)
        {
            if (properties == null || properties.Count != 5)
            {
                throw new ArgumentOutOfRangeException(nameof(properties));
            }

            InterfaceName = Encoding.Unicode.GetString(properties[2]);
            EventName = Encoding.Unicode.GetString(properties[3]);
            Parameters = Encoding.Unicode.GetString(properties[4]);
        }

        public EventCallMessage(string id, string interfaceName, string eventName, string parameters)
            : base(MessageType.EventCall, id)
        {
            InterfaceName = interfaceName;
            EventName = eventName;
            Parameters = parameters;
        }

        public override List<byte[]> GetProperties()
        {
            var properties = base.GetProperties();
            properties.Add(Encoding.Unicode.GetBytes(InterfaceName));
            properties.Add(Encoding.Unicode.GetBytes(EventName));
            properties.Add(Encoding.Unicode.GetBytes(Parameters));
            return properties;
        }

        public string InterfaceName { get; set; }
        public string EventName { get; set; }
        public string Parameters { get; set; }
    }
}
