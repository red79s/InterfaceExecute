using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialMessages;
using System;
using System.Collections.Generic;

namespace SerialMessageTests
{
    [TestClass]
    public class SerialMessageTests
    {
        [TestMethod]
        public void TestCreateMessage()
        {
            var dls = new DynamicLengthDataPackage();
            var message = dls.CreateMessage(new byte[] { 0x1, 0x2 });

            byte[] rm = null;
            dls.OnPackageReceived += (s, d) =>
            {
                rm = d;
            };

            dls.DataReceived(message);

            Assert.AreEqual(0x1, rm[0]);
            Assert.AreEqual(0x2, rm[1]);
        }

        [TestMethod]
        public void TestCreate2Messages()
        {
            var dls = new DynamicLengthDataPackage();
            var message1 = dls.CreateMessage(new byte[] { 0x1, 0x2 });
            var message2 = dls.CreateMessage(new byte[] { 0x4, 0x5 });

            List<byte[]> receivedMessages = new List<byte[]>();
            dls.OnPackageReceived += (s, d) =>
            {
                receivedMessages.Add(d);
            };

            var combindedMessages = new byte[message1.Length + message2.Length];
            Array.Copy(message1, combindedMessages, message1.Length);
            Array.Copy(message2, 0, combindedMessages, message1.Length, message2.Length);
            dls.DataReceived(combindedMessages);

            Assert.AreEqual(0x1, receivedMessages[0][0]);
            Assert.AreEqual(0x2, receivedMessages[0][1]);

            Assert.AreEqual(0x4, receivedMessages[1][0]);
            Assert.AreEqual(0x5, receivedMessages[1][1]);
        }
    }
}
