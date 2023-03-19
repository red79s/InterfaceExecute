using Eloe.InterfaceSerializer.DataPacket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using WebApiServer.Auth;

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
        public async Task<string> Execute([FromBody]FunctionDataPacket funcData)
        {
            try
            {
                var authHandler = new JwtAuthorizeHandler("https://auth.apx-systems.com/", new List<string> { "APX" });
                await authHandler.GetAuthServerConfiguration();

                //string executeArguments = "";
                //using (StreamReader stream = new StreamReader(Request.Body))
                //{
                //    executeArguments = stream.ReadToEndAsync().Result;
                //}

                //var funcData = JsonConvert.DeserializeObject<FunctionDataPacket>(executeArguments);
                //if (funcData == null)
                //{
                //    throw new Exception("Invalid arguments");
                //}

                var authHeader = Request.Headers.Authorization.ToString();
                if (authHeader != null)
                {
                    if (authHeader.StartsWith("bearer ", StringComparison.CurrentCultureIgnoreCase))
                    {
                        funcData.JwtToken = authHeader.Substring(7);
                    }
                }

                var rpcCollection = _interfaceRpcReceive.Get();
                var resultData = rpcCollection.HandleFunctionCall(funcData, authHandler);
                var resultString = JsonConvert.SerializeObject(resultData);
                return resultString;
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new FunctionReturnDataPacket { Exception = new Exception($"Unhandled exception: {ex}") });
            }
        }
    }
}
