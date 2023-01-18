using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SerialMessages
{
    public class DataMessageManager<T> : IDataMessage where T: IDataPackage, new()
    {
        public event EventHandler<IMessage> OnMessageReceived;

        private T _packageManager;
        public DataMessageManager()
        {
            _packageManager = new T();
            _packageManager.OnPackageReceived += PackageManager_OnPackageReceived;
        }

        private void PackageManager_OnPackageReceived(object sender, byte[] package)
        {
            var msg = DecodeMessage(package);
            OnMessageReceived?.Invoke(this, msg);
        }

        public void DataReceived(byte data)
        {
            _packageManager.DataReceived(data);
        }

        public void DataReceived(byte[] data)
        {
            _packageManager.DataReceived(data);
        }

        private IMessage DecodeMessage(byte[] data)
        {
            var properties = MessageUtils.DecodeProperties(data);
            if (properties.Count < 1)
                return null;

            var messageType = (MessageType)BitConverter.ToInt32(properties[0], 0);
            switch (messageType)
            {
                case MessageType.Ping:
                    return new PingMessage(properties);
                case MessageType.FunctionCall:
                    return new FunctionCallMessage(properties);
                case MessageType.FunctionReturn:
                    return new FunctionCallReturnMessage(properties);
                case MessageType.Authenticate:
                    break;
               
            }
            return null;
        }

        public byte[] EncodeMessage(IMessage message)
        {
            var messageData = MessageUtils.EncodeProperties(message.GetProperties());
            return _packageManager.CreateMessage(messageData);
        }
    }
}
