using Eloe.InterfaceSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Eloe.InteraceSerializerTests
{
    [TestClass]
    public class ParameterConvertTests
    {
        [TestMethod]
        public void TestSerializeStringParam()
        {
            var serializer = new ParameterConvert();
            var res = serializer.Serialize("paramName", typeof(string), "val");
        }

        [TestMethod]
        public void TestDeserializeStringParam()
        {
            var serializer = new ParameterConvert();
            var res = serializer.Deserialize("paramName", typeof(string), "{\"paramName\":\"val\"}");
        }

        [TestMethod]
        public void SerializeString()
        {
            var jsonStr = JsonConvert.SerializeObject("abc");
            var res = JsonConvert.DeserializeObject(jsonStr, typeof(string));
        }
    }
}
