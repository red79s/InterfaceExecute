

using Eloe.InterfaceRpc;
using Eloe.InterfaceSerializer.DataPacket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace WebApiServer.Controllers
{
    public class InterfaceExecuteController : Controller
    {
        private readonly InterfaceExecuteFactory _interfaceRpcReceive;

        public InterfaceExecuteController(InterfaceExecuteFactory interfaceRpcReceive)
        {
            _interfaceRpcReceive = interfaceRpcReceive;
        }

        //[Authorize]
        [HttpPost("~/InterfaceExecute/Execute")]
        public string Execute()
        {
            try
            {
                string executeArguments = "";
                using (StreamReader stream = new StreamReader(Request.Body))
                {
                    executeArguments = stream.ReadToEndAsync().Result;
                }

                var funcData = JsonConvert.DeserializeObject<FunctionDataPacket>(executeArguments);
                if (funcData == null)
                {
                    throw new Exception("Invalid arguments");
                }
                var rpcCollection = _interfaceRpcReceive.Get();
                var resultData = rpcCollection.HandleFunctionCall(funcData);
                var resultString = JsonConvert.SerializeObject(resultData);
                return resultString;
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new FunctionReturnDataPacket { Exception = $"Unhandled exception: {ex}" });
            }
        }
    }
}
