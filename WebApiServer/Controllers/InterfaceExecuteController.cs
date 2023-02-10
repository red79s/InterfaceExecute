

using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer.DataPacket;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApiServer.Controllers
{
    public class InterfaceExecuteController : Controller
    {
        private readonly InterfaceExecuteFactory _interfaceRpcReceive;

        public InterfaceExecuteController(InterfaceExecuteFactory interfaceRpcReceive)
        {
            _interfaceRpcReceive = interfaceRpcReceive;
        }

        [HttpPost("~/InterfaceExecute/Execute")]
        public string Execute([FromBody]string executeArguments)
        {
            var funcData = JsonConvert.DeserializeObject<FunctionDataPacket>(executeArguments);
            var col = _interfaceRpcReceive.Get();
            var resultData = col.HandleFunctionCall(funcData);
            var resultString = JsonConvert.SerializeObject(resultData);
            return resultString;
        }
    }
}
