using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerialMessageTests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void TestPingEncodeDecode()
        {
            var msgManager = new DataMessageManager<DynamicLengthDataPackage>();
            IPingMessage msgOut = null;
            msgManager.OnMessageReceived += (s, m) =>
            {
                msgOut = (IPingMessage)m;
            };

            var msg = new PingMessage("1", 23);
            var data = msgManager.EncodeMessage(msg);
            msgManager.DataReceived(data);
            Assert.AreEqual(MessageType.Ping, msgOut.MessageType);
            Assert.AreEqual("1", msgOut.Id);
            Assert.AreEqual(23, msgOut.Counter);
        }

        [TestMethod]
        public void TestFunctionCallEncodeDecode()
        {
            var msgManager = new DataMessageManager<DynamicLengthDataPackage>();
            IFunctionCallMessage msgOut = null;
            msgManager.OnMessageReceived += (s, m) =>
            {
                msgOut = (IFunctionCallMessage)m;
            };

            var msg = new FunctionCallMessage("345", "ISomeInterface", "foo", "{abcø}");
            var data = msgManager.EncodeMessage(msg);
            msgManager.DataReceived(data);
            Assert.AreEqual(MessageType.FunctionCall, msgOut.MessageType);
            Assert.AreEqual("345", msgOut.Id);
            Assert.AreEqual("ISomeInterface", msgOut.InterfaceName);
            Assert.AreEqual("foo", msgOut.FunctionName);
            Assert.AreEqual("{abcø}", msgOut.Parameters);
        }
    }
}
