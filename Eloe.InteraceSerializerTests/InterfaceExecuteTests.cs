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
            Assert.AreEqual("Eloe.InteraceSerializerTests.ITestInterface", ctx.InterfaceFullName);
            Assert.AreEqual("{\"item\":\"a\"}", ctx.Payload);
            Assert.IsFalse(ctx.HaveReturnValue);
        }

        [TestMethod]
        public void TestDeserializeInit()
        {
            var impl = new TestInterfaceImpl();
            var executer = new InterfaceExecute<ITestInterface>(impl);
            executer.Execute("Init:0", "{}");
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
                context.ReturnValue = "{\"ReturnValue\": \"abc\"}";
            };
            var i = executer.GetInterface();

            var res = i.GetItem("a");
            Assert.AreEqual("abc", res);
        }

        [TestMethod]
        public void TestSerializeGetItemOverload()
        {
            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                context.ReturnValue = "{\"ReturnValue\": \"abc\"}";
            };
            var i = executer.GetInterface();

            var res = i.GetItem("key1", new SomeReq { Id = 2, Name = "name"});
            Assert.AreEqual("abc", res);
        }

        [TestMethod]
        public void TestDeserializeGetItem()
        {
            var impl = new TestInterfaceImpl();
            impl.SetItem("a");
            var executer = new InterfaceExecute<ITestInterface>(impl);
            var res = executer.Execute("GetItem:0", "{\"itemKey\": \"a\"}");
            Assert.AreEqual("{\"ReturnValue\": \"a\"}", res);
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
                context.ReturnValue = "{\"ReturnValue\": true}";
            };
            var i = executer.GetInterface();

            var res = i.SendMessage("a", 1);
            res.Wait();
            var a = res.Result;
            Assert.AreEqual(true, a);
        }

        [TestMethod]
        public void TestDeserializeTaskOfBoolReturnValue()
        {
            var executer = new InterfaceExecute<ITestInterface>(new TestInterfaceImpl());
            var res = executer.Execute("SendMessage:1", "{\"message\":\"hello\", \"num\": 23}");
            Assert.AreEqual("{\"ReturnValue\": \"True\"}", res);
        }

        [TestMethod]
        public void TestDeserializeTaskReturnValue()
        {
            var executer = new InterfaceExecute<ITestInterface>(new TestInterfaceImpl());
            var res = executer.Execute("SendMessage:0", "{\"message\":\"hello\"}");
            Assert.IsNull(res);
        }
    }
}
