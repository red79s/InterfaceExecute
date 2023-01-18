using System;
using System.Text;

namespace Atec.InterfaceSerializer.DataPacket
{
    public class FunctionReturnDataPacketEncoding : IFunctionReturnDataPacketEncoding
    {
        public FunctionReturnDataPacketEncoding()
        {
        }

        public byte[] Encode(int id, string returnValue, string exception)
        {
            if (returnValue == null)
            {
                returnValue = "";
            }
            if (exception == null)
            {
                exception = "";
            }

            var idEncoded = BitConverter.GetBytes(id);
            var returnValueEncoded = Encoding.UTF8.GetBytes(returnValue);
            var exceptionEncoded = Encoding.UTF8.GetBytes(exception);
            var buffer = new byte[idEncoded.Length +
                returnValueEncoded.Length +
                exceptionEncoded.Length +
                8];

            int index = AddData(buffer, idEncoded, 0);
            index = AddData(buffer, BitConverter.GetBytes(returnValueEncoded.Length), index);
            index = AddData(buffer, returnValueEncoded, index);
            index = AddData(buffer, BitConverter.GetBytes(exceptionEncoded.Length), index);
            AddData(buffer, exceptionEncoded, index);

            return buffer;
        }

        private int AddData(byte[] buffer, byte[] data, int index)
        {
            Array.Copy(data, 0, buffer, index, data.Length);

            return index + data.Length;
        }

        public FunctionReturnDataPacketInfo Decode(byte[] data)
        {
            int index = 0;
            var id = BitConverter.ToInt32(data, index);
            index += 4;
            var returnValueLength = BitConverter.ToInt32(data, 4);
            index += 4;
            var returnValue = GetString(data, index, returnValueLength);
            index += returnValueLength;
            var exceptionLength = BitConverter.ToInt32(data, index);
            index += 4;
            var exception = GetString(data, index, exceptionLength);


            return new FunctionReturnDataPacketInfo
            {
                Id = id,
                ReturnValue = returnValue,
                Exception = exception
            };
        }

        private string GetString(byte[] buffer, int index, int length)
        {
            var strEncoded = new byte[length];
            Array.Copy(buffer, index, strEncoded, 0, length);
            return Encoding.UTF8.GetString(strEncoded);
        }
    }
}
