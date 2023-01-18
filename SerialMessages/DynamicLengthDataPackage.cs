using System;
using System.Collections.Generic;
using System.Linq;

namespace SerialMessages
{
    public class DynamicLengthDataPackage : IDataPackage
    {
        public event EventHandler<byte[]> OnPackageReceived;

        const int _messageHeader = 0x84FA;

        private readonly List<byte> _receiveBuffer = new List<byte>();

        public byte[] CreateMessage(byte[] data)
        {
            var outBuffer = new byte[8 + data.Length];
            var headerBytes = BitConverter.GetBytes(_messageHeader);
            var lengthBytes = BitConverter.GetBytes(data.Length);
            Array.Copy(headerBytes, 0, outBuffer, 0, 4);
            Array.Copy(lengthBytes, 0, outBuffer, 4, 4);
            Array.Copy(data, 0, outBuffer, 8, data.Length);
            return outBuffer;
        }

        public void DataReceived(byte data)
        {
            _receiveBuffer.Add(data);
            ProcessReceivedData();
        }

        public void DataReceived(byte[] data)
        {
            _receiveBuffer.AddRange(data);
            ProcessReceivedData();
        }

        private void ProcessReceivedData()
        {
            if (_receiveBuffer.Count < 8)
                return;

            var headerBuffer = _receiveBuffer.Take(8).ToArray();
            var messageHeader = BitConverter.ToInt32(headerBuffer, 0);
            var length = BitConverter.ToInt32(headerBuffer, 4);

            if (messageHeader != _messageHeader)
                throw new Exception($"Invalid data received: {headerBuffer}");

            if (_receiveBuffer.Count >= (length + 8))
            {
                _receiveBuffer.RemoveRange(0, 8);
                var data = _receiveBuffer.Take(length).ToArray();
                _receiveBuffer.RemoveRange(0, length);
                OnPackageReceived?.Invoke(this, data);

                ProcessReceivedData();
            }
        }
    }
}
