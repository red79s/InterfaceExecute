using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientServerCom
{
    public interface ITestInterface
    {
        event EventHandler<string> OnEventTriggered;
        void Foo();
        void Bar(int i);
        string GetVal(int i);

        TestClassA GetTestClass(TestClassA param);
    }

    public class TestClassA
    {
        public string Name { get; set; }
        public float Val { get; set; }
    }
}
