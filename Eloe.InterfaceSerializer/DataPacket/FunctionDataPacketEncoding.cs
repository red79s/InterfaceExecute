using System;
using System.Collections.Generic;
using System.Text;

namespace Eloe.InterfaceSerializer.DataPacket
{
    public class FunctionDataPacketEncoding : IFunctionDataPacketEncoding
    {
        public FunctionDataPacketEncoding()
        {
        }

        public byte[] Encode(int id, string className, string functionName, string functionParameters)
        {
            var idEncoded = BitConverter.GetBytes(id);
            var classNameEncoded = Encoding.UTF8.GetBytes(className);
            var functionNameEncoded = Encoding.UTF8.GetBytes(functionName);
            var functionParametersEncoded = Encoding.UTF8.GetBytes(functionParameters);
            var buffer = new byte[idEncoded.Length +
                classNameEncoded.Length +
                functionNameEncoded.Length +
                functionParametersEncoded.Length +
                12];

            int index = AddData(buffer, idEncoded, 0);
            index = AddData(buffer, BitConverter.GetBytes(classNameEncoded.Length), index);
            index = AddData(buffer, classNameEncoded, index);
            index = AddData(buffer, BitConverter.GetBytes(functionNameEncoded.Length), index);
            index = AddData(buffer, functionNameEncoded, index);
            index = AddData(buffer, BitConverter.GetBytes(functionParametersEncoded.Length), index);
            AddData(buffer, functionParametersEncoded, index);

            return buffer;
        }

        private int AddData(byte[] buffer, byte[] data, int index)
        {
            Array.Copy(data, 0, buffer, index, data.Length);

            return index + data.Length;
        }

        public FunctionDataPacket Decode(byte[] data)
        {
            int index = 0;
            var id = BitConverter.ToInt32(data, index);
            index += 4;
            var classNameLength = BitConverter.ToInt32(data, 4);
            index += 4;
            var className = GetString(data, index, classNameLength);
            index += classNameLength;
            var functionNameLength = BitConverter.ToInt32(data, index);
            index += 4;
            var functionName = GetString(data, index, functionNameLength);
            index += functionNameLength;
            var functionParametersLength = BitConverter.ToInt32(data, index);
            index += 4;
            var functionParameters = GetString(data, index, functionParametersLength);

            return new FunctionDataPacket
            {
                Id = id,
                ClassName = className,
                FunctionName = functionName,
                FunctionParameters = functionParameters
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
