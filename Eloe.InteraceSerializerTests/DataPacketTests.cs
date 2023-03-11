using Eloe.InterfaceSerializer.DataPacket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Eloe.InteraceSerializerTests
{
    [TestClass]
    public class DataPacketTests
    {
        [TestMethod]
        public void EncodeAndDecodeEmptyPacket()
        {
            var dp = new DataPacketEncoding();
            var buffer = dp.Encode(DataPacketType.FunctionCall, new byte[0]);
            Assert.AreEqual(10, buffer.Length);
            Assert.AreEqual(10, BitConverter.ToInt32(buffer, 1));
            var dataInfo = dp.Decode(buffer);
            Assert.IsNotNull(dataInfo.Data);
            Assert.AreEqual(0, dataInfo.Data.Length);
            Assert.AreEqual(DataPacketType.FunctionCall, dataInfo.PackageType);
        }

        [TestMethod]
        public void EncodeAndDecodeWithData()
        {
            var data = new byte[] { 1, 2, 3 };
            var dp = new DataPacketEncoding();
            var buffer = dp.Encode(DataPacketType.FunctionCall, data);
            Assert.AreEqual(13, buffer.Length);
            Assert.AreEqual(13, BitConverter.ToInt32(buffer, 1));

            var dataInfo = dp.Decode(buffer);
            Assert.IsNotNull(dataInfo.Data);
            Assert.AreEqual(3, dataInfo.Data.Length);
            Assert.AreEqual(2, dataInfo.Data[1]);
            Assert.AreEqual(DataPacketType.FunctionCall, dataInfo.PackageType);
        }

        [TestMethod]
        public void EncodeAndDecodeFunctionNoData()
        {
            var fdp = new FunctionDataPacketEncoding();
            var buffer = fdp.Encode(new FunctionDataPacket { Id = 55, ClassName = "", FunctionName = "", FunctionParameters = null });
            
            var fInfo = fdp.Decode(buffer);
            Assert.AreEqual(55, fInfo.Id);
            Assert.AreEqual("", fInfo.ClassName);
            Assert.AreEqual("", fInfo.FunctionName);
            Assert.AreEqual(null, fInfo.FunctionParameters);
        }

        [TestMethod]
        public void EncodeAndDecodeFunction()
        {
            var fdp = new FunctionDataPacketEncoding();
            var buffer = fdp.Encode(new FunctionDataPacket { Id = 55, ClassName = "ClassA", FunctionName = "Func_B", FunctionParameters = new List<byte[]> { new byte[] { 30, 32 } } });

            var fInfo = fdp.Decode(buffer);
            Assert.AreEqual(55, fInfo.Id);
            Assert.AreEqual("ClassA", fInfo.ClassName);
            Assert.AreEqual("Func_B", fInfo.FunctionName);
            Assert.AreEqual(32, fInfo.FunctionParameters[0][1]);
        }
    }
}
