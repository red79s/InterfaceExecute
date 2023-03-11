using System;

namespace Eloe.InterfaceSerializer.DataPacket
{
    public class FunctionReturnDataPacket
    {
        public int Id { get; set; }
        public byte[] ReturnValue { get; set; }
        public Exception Exception { get; set; }
    }
}
