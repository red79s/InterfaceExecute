using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessages
{
    public class MessageUtils
    {
        public static byte[] EncodeProperties(List<byte[]> properties)
        {
            var bufferSize = (properties.Count * 4) + 4;
            foreach (var prop in properties)
            {
                bufferSize += prop.Length;
            }

            var buffer = new byte[bufferSize];
            CopyIntToArray(properties.Count, buffer, 0);
            var bufferIndex = 4;
            for (int i = 0; i < properties.Count; i++)
            {
                CopyIntToArray(properties[i].Length, buffer, bufferIndex);
                bufferIndex += 4;
                Array.Copy(properties[i], 0, buffer, bufferIndex, properties[i].Length);
                bufferIndex += properties[i].Length;
            }

            return buffer;
        }

        public static List<byte[]> DecodeProperties(byte[] buffer)
        {
            if (buffer.Length < 4)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            int numProperties = BitConverter.ToInt32(buffer, 0);
            int bufferIndex = 4;
            var properties = new List<byte[]>();
            for (int i = 0; i < numProperties; i++)
            {
                var length = BitConverter.ToInt32(buffer, bufferIndex);
                bufferIndex += 4;
                var propBuffer = new byte[length];
                Array.Copy(buffer, bufferIndex, propBuffer, 0, length);
                properties.Add(propBuffer);
                bufferIndex += length;
            }

            return properties;
        }

        static void CopyIntToArray(int val, byte[] buffer, int index)
        {
            if (buffer.Length < index + 4)
                throw new ArgumentOutOfRangeException(nameof(index));

            var bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, buffer, index, 4);
        }
    }
}
