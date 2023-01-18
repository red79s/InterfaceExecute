using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public class FunctionCallMessage : Message, IFunctionCallMessage
    {
        public FunctionCallMessage(List<byte[]> properties) 
            : base(properties)
        {
            if (properties == null || properties.Count != 5)
            {
                throw new ArgumentOutOfRangeException(nameof(properties));
            }

            InterfaceName = Encoding.Unicode.GetString(properties[2]);
            FunctionName = Encoding.Unicode.GetString(properties[3]);
            Parameters = Encoding.Unicode.GetString(properties[4]);
        }

        public FunctionCallMessage(string id, string interfaceName, string functionName, string parameters)
            : base (MessageType.FunctionCall, id)
        {
            InterfaceName = interfaceName;
            FunctionName = functionName;
            Parameters = parameters;
        }

        public override List<byte[]> GetProperties()
        {
            var properties = base.GetProperties();
            properties.Add(Encoding.Unicode.GetBytes(InterfaceName));
            properties.Add(Encoding.Unicode.GetBytes(FunctionName));
            properties.Add(Encoding.Unicode.GetBytes(Parameters));
            return properties;
        }

        public string InterfaceName { get; set; }
        public string FunctionName { get; set; }
        public string Parameters { get; set; }
    }
}
