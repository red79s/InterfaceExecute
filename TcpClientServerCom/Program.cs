using SerialMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpClientServerCom
{
    class Program
    {
        static void Main(string[] args)
        {
            var testImpl = new TestInterfaceImpl();
            var server = new TcpMessageServer<DataMessageManager<DynamicLengthDataPackage>>(6666);
            server.ImplementInterface<ITestInterface>(testImpl);
            server.Start();


            var client = new TcpMessageClient<DataMessageManager<DynamicLengthDataPackage>>();
            var i = client.ImplementInterface<ITestInterface>();
            client.Connect("localhost", 6666);

            i.OnEventTriggered += (s, e) => Console.WriteLine($"recived callback: {e}");

            i.Foo();
            i.Bar(33);
            var str = i.GetVal(42);
            Console.WriteLine($"Return value is: {str}");

            var ret = i.GetTestClass(new TestClassA { Name = "a", Val = 3.4f });
            Console.WriteLine($"Return value is {ret.Name}, {ret.Val}");

            testImpl.TriggerEvent("eee");

            var k = Console.ReadKey();
        }
    }
}
