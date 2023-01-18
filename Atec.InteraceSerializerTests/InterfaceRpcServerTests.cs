﻿using Atec.InterfaceSerializer;
using Atec.InterfaceSerializer.DataPacket;
using Eloe.InterfaceRpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atec.InteraceSerializerTests
{
    [TestClass]
    public class InterfaceRpcServerTests
    {
        private Mock<IInterfaceComunicationChannelServer> _communicationChannelMock = new Mock<IInterfaceComunicationChannelServer>();
        private Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private IDataPacketFactory _dataPacketFactory;

        [TestInitialize]
        public void Init()
        {
            _dataPacketFactory = new DataPacketFactory(
                new DataPacketEncoding(), 
                new FunctionDataPacketEncoding(), 
                new FunctionReturnDataPacketEncoding());
        }

        private InterfaceRpcServer CreateServer()
        {
            return new InterfaceRpcServer(_communicationChannelMock.Object, _dataPacketFactory, _loggerMock.Object);
        }

        [TestMethod]
        public void TestSendMessage()
        {
            var server = CreateServer();
            var impl = new TestInterfaceImpl();
            server.ImplementInterface<ITestInterface>(impl);

            var package = _dataPacketFactory.CreateFunctionCall(1, typeof(ITestInterface).FullName, "SetItem", "{\"item\": \"abc\"}");
            _communicationChannelMock.Raise(x => x.OnMessageReceived += null, null, new MessageReceivedArgs { Data = package });

            _communicationChannelMock.Verify(x => x.SendAsync(It.IsAny<string>(), It.IsAny<byte[]>()));
        }

        [TestMethod]
        public void TestInvokeClientFunction()
        {
            var server = CreateServer();
            var cliInterface = server.AddClientCallbackInterface<ITestInterface>();
            var functionReturned = false;
            Task.Run(() =>
            {
                cliInterface.SetItem("ab c");
                functionReturned = true;
            });
            
            _communicationChannelMock.Verify(x => x.SendAsync(It.IsAny<string>(), It.IsAny<byte[]>()));
            var returnPackage = _dataPacketFactory.CreateFuctionReturnCall(1, "", "");
            _communicationChannelMock.Raise(x => x.OnMessageReceived += null, null, new MessageReceivedArgs { Data = returnPackage });
            Thread.Sleep(1000);
            Assert.IsTrue(functionReturned);
        }
    }
}