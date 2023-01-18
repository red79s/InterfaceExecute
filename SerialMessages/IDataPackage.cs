using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public interface IDataPackage
    {
        event EventHandler<byte[]> OnPackageReceived;
        void DataReceived(byte data);
        void DataReceived(byte[] data);
        byte[] CreateMessage(byte[] data);
    }
}
