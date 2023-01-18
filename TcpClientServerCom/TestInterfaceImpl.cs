using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientServerCom
{
    public class TestInterfaceImpl : ITestInterface
    {
        public event EventHandler<string> OnEventTriggered;

        public void Bar(int i)
        {
            Console.WriteLine($"Called Bar() i={i}");
        }

        public void Foo()
        {
            Console.WriteLine("Called Foo()");
        }

        public TestClassA GetTestClass(TestClassA param)
        {
            return new TestClassA { Name = param.Name + " mod", Val = param.Val };
        }

        public string GetVal(int i)
        {
            return $"Value is {i}";
        }

        public void TriggerEvent(string val)
        {
            OnEventTriggered?.Invoke(this, val);
        }
    }
}
