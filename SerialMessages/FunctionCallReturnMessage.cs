using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public class FunctionCallReturnMessage : Message, IFunctionCallReturnMessage
    {
        public FunctionCallReturnMessage(List<byte[]> properties)
            : base(properties)
        {
            if (properties == null || properties.Count != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(properties));
            }

            ReturnValue = Encoding.Unicode.GetString(properties[2]);
        }

        public FunctionCallReturnMessage(string id, string returnValue)
            : base(MessageType.FunctionReturn, id)
        {
            ReturnValue = returnValue;
        }

        public override List<byte[]> GetProperties()
        {
            var properties = base.GetProperties();
            properties.Add(Encoding.Unicode.GetBytes(ReturnValue));
            return properties;
        }
        public string ReturnValue { get; set; }
    }
}
