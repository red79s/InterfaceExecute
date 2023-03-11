using Eloe.InterfaceSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Assert.AreEqual(1, ctx.MethodParameters.Count);
            Assert.IsFalse(ctx.HaveReturnValue);
        }

        [TestMethod]
        public void TestDeserializeInit()
        {
            var impl = new TestInterfaceImpl();
            var executer = new InterfaceExecute<ITestInterface>(impl);
            executer.Execute("Init:0", new List<byte[]>());
            Assert.IsTrue(impl.InitCalled);
        }

        [TestMethod]
        public void TestSerializeGetItem()
        {
            var options = new InterfaceSerializerOptions();

            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                context.ReturnValue = options.ParameterSerializer.Serialize("", typeof(string), "abc");
            };
            var i = executer.GetInterface();

            var res = i.GetItem("a");
            Assert.AreEqual("abc", res);
        }

        [TestMethod]
        public void TestSerializeGetItemOverload()
        {
            var options = new InterfaceSerializerOptions();

            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                context.ReturnValue = options.ParameterSerializer.Serialize("ReturnValue", typeof(string), "abc");
            };
            var i = executer.GetInterface();

            var res = i.GetItem("key1", new SomeReq { Id = 2, Name = "name"});
            Assert.AreEqual("abc", res);
        }

        [TestMethod]
        public void TestDeserializeGetItem()
        {
            var options = new InterfaceSerializerOptions();

            var impl = new TestInterfaceImpl();
            impl.SetItem("a");
            var executer = new InterfaceExecute<ITestInterface>(impl);
            var res = executer.Execute("GetItem:0", new List<byte[]> { options.ParameterSerializer.Serialize("itemKey", typeof(string), "a") });
            Assert.AreEqual("a", options.ParameterSerializer.Deserialize("", typeof(string), res));
        }

        [TestMethod]
        public void TestSerializeTaskReturnValue()
        {
            var options = new InterfaceSerializerOptions();

            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                context.ReturnValue = null;
            };
            var i = executer.GetInterface();

            var res = i.SendMessage("a");
            res.Wait();
        }

        [TestMethod]
        public void TestSerializeTaskOfBoolReturnValue()
        {
            var options = new InterfaceSerializerOptions();
            SerializedExecutionContext ctx = null;
            var executer = new InterfaceExecute<ITestInterface>();
            executer.OnExecute += (sender, context) =>
            {
                ctx = context;
                context.ReturnValue = options.ParameterSerializer.Serialize("ReturnValue", typeof(bool), true);
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
            var options = new InterfaceSerializerOptions();
            var executer = new InterfaceExecute<ITestInterface>(new TestInterfaceImpl());
            var res = executer.Execute("SendMessage:1", new List<byte[]>
            {
                options.ParameterSerializer.Serialize("message", typeof(string), "hello"),
                options.ParameterSerializer.Serialize("num", typeof(int), 23)
            });
            Assert.AreEqual(true, options.ParameterSerializer.Deserialize("", typeof(bool), res));
        }

        [TestMethod]
        public void TestDeserializeTaskReturnValue()
        {
            var options = new InterfaceSerializerOptions();
            var executer = new InterfaceExecute<ITestInterface>(new TestInterfaceImpl());
                var res = executer.Execute("SendMessage:0", new List<byte[]>
                {
                    options.ParameterSerializer.Serialize("message", typeof(string), "hello")
                });
            Assert.IsNull(res);
        }

        [TestMethod]
        public void TestAuthorize()
        {
            var options = new InterfaceSerializerOptions();
            var authHandler = new AuthHandlerTest("abc", new List<string> { "Admin", "User" });
            var executer = new InterfaceExecute<ITestInterface>(new TestInterfaceImpl());
            var res = executer.Execute("WithAuth:0", new List<byte[]>
                {
                    options.ParameterSerializer.Serialize("num", typeof(int), 10)
                }, 
                "abc", authHandler);
            Assert.IsNotNull(res);
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestAuthorizeFail()
        {
            var options = new InterfaceSerializerOptions();
            var authHandler = new AuthHandlerTest("abc", new List<string> { "Admin", "User" });
            var executer = new InterfaceExecute<ITestInterface>(new TestInterfaceImpl());
            var res = executer.Execute("WithAuth:0", new List<byte[]>
                {
                    options.ParameterSerializer.Serialize("num", typeof(int), 10)
                },
                "abcd", authHandler);
            Assert.IsNotNull(res);
        }

        public class AuthHandlerTest : IAuthorizeHandler
        {
            private readonly string _validJwtToken;
            private readonly List<string> _allowedRoles;

            public AuthHandlerTest(string validJwtToken, List<string> allowedRoles)
            {
                _validJwtToken = validJwtToken;
                _allowedRoles = allowedRoles;
            }

            public void Authorize(string jwtToken)
            {
                if (jwtToken != _validJwtToken)
                {
                    throw new Exception("Not authorized");
                }
            }

            public void Authorize(string jwtToken, List<string> roles)
            {
                Authorize(jwtToken);

                foreach (var role in roles) 
                {
                    if (_allowedRoles == null || _allowedRoles.FirstOrDefault(x => x == role) == null)
                    {
                        throw new Exception($"Not authorized, not access to role: {role}");
                    }
                }
            }
        }
    }
}
