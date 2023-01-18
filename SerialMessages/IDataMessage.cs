using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public interface IDataMessage
    {
        event EventHandler<IMessage> OnMessageReceived;
        void DataReceived(byte data);
        void DataReceived(byte[] data);
        byte[] EncodeMessage(IMessage message);
    }
}
