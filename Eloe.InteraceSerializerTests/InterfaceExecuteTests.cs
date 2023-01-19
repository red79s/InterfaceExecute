using Eloe.InterfaceSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eloe.InteraceSerializerTests
{
    [TestClass]
    public class InterfaceExecuteTests
    {
        [TestMethod]
        public void TestSerializeInit()
        {
            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) => { ctx = context; };
            var i = executer.GetInterface();
            i.Init();

            Assert.AreEqual("Init", ctx.MethodName);
            Assert.IsFalse(ctx.HaveReturnValue);
        }

        [TestMethod]
        public void TestSerializeSetProduct()
        {
            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) => { ctx = context; };
            var i = executer.GetInterface();
            i.SetItem("a");

            Assert.AreEqual("SetItem", ctx.MethodName);
            Assert.AreEqual("SetItem", ctx.ExecutePath);
            Assert.AreEqual("{\"item\":\"a\"}", ctx.Payload);
            Assert.IsFalse(ctx.HaveReturnValue);
        }

        [TestMethod]
        public void TestDeserializeInit()
        {
            var impl = new TestInterfaceImpl();
            var executer = new InterfaceExecute<ITestInterface>(impl);
            executer.Execute("Init", "{}");
            Assert.IsTrue(impl.InitCalled);
        }

        [TestMethod]
        public void TestSerializeGetItem()
        {
            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                //context.ReturnValue = "{\"ReturnValue\": \"abc\"}";
            };
            var i = executer.GetInterface();

            var res = i.GetItem("a");
        }

        [TestMethod]
        public void TestDeserializeGetItem()
        {
            var impl = new TestInterfaceImpl();
            impl.SetItem("a");
            var executer = new InterfaceExecute<ITestInterface>(impl);
            var res = executer.Execute("GetItem", "{\"itemKey\": \"a\"}");
        }

        [TestMethod]
        public void TestSerializeTaskReturnValue()
        {
            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                context.ReturnValue = "";
            };
            var i = executer.GetInterface();

            var res = i.SendMessage("a");
            res.Wait();
        }

        [TestMethod]
        public void TestSerializeTaskOfBoolReturnValue()
        {
            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                context.ReturnValue = "";
            };
            var i = executer.GetInterface();

            var res = i.SendMessage("a", 1);
            res.Wait();
            var a = res.Result;
            Assert.AreEqual(true, a);
        }
    }
}
