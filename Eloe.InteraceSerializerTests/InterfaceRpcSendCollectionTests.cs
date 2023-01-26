using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eloe.InteraceSerializerTests
{
    [TestClass]
    public class InterfaceRpcSendCollectionTests
    {
        private Mock<ILogger> _logger = new Mock<ILogger>();

        [TestMethod]
        public void TestCancelExecution()
        {
            var dataPacketEncoder = new DataPacketEncoding();
            var functionDataPacketEncoder = new FunctionDataPacketEncoding();
            var functionReturnDataPacketEncoder = new FunctionReturnDataPacketEncoding();
            var dataPacketFactory = new DataPacketFactory(dataPacketEncoder, functionDataPacketEncoder, functionReturnDataPacketEncoder);
            var col = new InterfaceRpcSendCollection(dataPacketFactory, _logger.Object);
            var interf = col.AddProxyCallbackInterface<ITestInterface>();
            try
            {
                var task = interf.SendMessage("abc");
                Thread.Sleep(1000);
                col.AbortAllExecution();
                task.Wait();
            }
            catch (Exception ex)
            {

            }
            

        }
    }
}
