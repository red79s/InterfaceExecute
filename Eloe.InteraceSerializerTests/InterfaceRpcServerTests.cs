using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eloe.InteraceSerializerTests
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
            return new InterfaceRpcServer(_communicationChannelMock.Object, _loggerMock.Object, true);
        }

        [TestMethod]
        public void TestSendMessage()
        {
            var options = new InterfaceSerializerOptions();
            var server = CreateServer();
            var impl = new TestInterfaceImpl();
            server.ImplementInterface<ITestInterface>(impl);

            var package = _dataPacketFactory.CreateFunctionCall(1, typeof(ITestInterface).FullName, "SetItem", new List<byte[]>
                {
                    options.ParameterSerializer.Serialize("item", typeof(string), "abc")
                });
            _communicationChannelMock.Raise(x => x.OnMessageReceived += null, null, new MessageReceivedServerArgs { Data = package });

            _communicationChannelMock.Verify(x => x.SendAsync(It.IsAny<string>(), It.IsAny<byte[]>()));
        }

        [TestMethod]
        public void TestInvokeClientFunction()
        {
            var server = CreateServer();
            var cliInterface = server.AddClientCallbackInterface<ITestInterface>("client1");
            var functionReturned = false;
            Task.Run(() =>
            {
                cliInterface.SetItem("ab c");
                functionReturned = true;
            });

            _communicationChannelMock.Verify(x => x.SendAsync(It.IsAny<string>(), It.IsAny<byte[]>()));
            var returnPackage = _dataPacketFactory.CreateFuctionReturnCall(1, null, null);
            _communicationChannelMock.Raise(x => x.OnMessageReceived += null, null, new MessageReceivedServerArgs { Data = returnPackage });
            Thread.Sleep(1000);
            Assert.IsTrue(functionReturned);
        }
    }
}
