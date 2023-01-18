using System;

namespace Atec.InterfaceSerializer.DataPacket
{
    public enum DataPacketType
    {
        Unknown = 0,
        FunctionCall = 1,
        FunctionReturn = 2,
        FunctionReturnException = 3
    }

    public class DataPacketEncoding : IDataPacketEncoding
    {
        public const int FixedPackageLength = 10;
        public byte[] Encode(DataPacketType packetType, byte[] data)
        {
            var packageLength = data.Length + FixedPackageLength;
            var buffer = new byte[packageLength];
            buffer[0] = (byte)0x02;
            int index = 1;
            Array.Copy(BitConverter.GetBytes(packageLength), 0, buffer, index, 4);
            index += 4;
            Array.Copy(BitConverter.GetBytes((int)packetType), 0, buffer, index, 4);
            index += 4;
            Array.Copy(data, 0, buffer, index, data.Length);
            buffer[buffer.Length - 1] = (byte)0x03;
            return buffer;
        }

        public DataPacketInfo Decode(byte[] buffer)
        {
            if (buffer == null || buffer.Length < FixedPackageLength)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer[0] != 0x02 || buffer[buffer.Length - 1] != 0x03)
                throw new Exception("Invalid data package, missing STX or and ETX");

            var packageLength = BitConverter.ToInt32(buffer, 1);

            var packageType = (DataPacketType)BitConverter.ToInt32(buffer, 5);
            var data = new byte[packageLength - FixedPackageLength];
            Array.Copy(buffer, 9, data, 0, packageLength - FixedPackageLength);

            return new DataPacketInfo { PackageType = packageType, Data = data };
        }
    }
}
