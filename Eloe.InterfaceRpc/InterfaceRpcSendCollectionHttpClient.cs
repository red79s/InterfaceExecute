using Eloe.InterfaceSerializer;
using Eloe.InterfaceSerializer.DataPacket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Eloe.InterfaceRpc
{
    public class InterfaceRpcSendCollectionHttpClient
    {
        private readonly string _url;
        private List<IInterfaceExecute> _proxyInterfaces = new List<IInterfaceExecute>();
        private HttpClient _httpClient;
        public InterfaceRpcSendCollectionHttpClient(string baseUrl, string bearerToken)
        {
            _url = baseUrl + "/InterfaceExecute/Execute";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", bearerToken);
        }

        public U AddProxyCallbackInterface<U>() where U : class
        {
            var iExeExisting = (InterfaceExecute<U>)_proxyInterfaces.FirstOrDefault(x => x.InterfaceType == typeof(U));
            if (iExeExisting != null)
                return iExeExisting.GetInterface();

            var iExec = new InterfaceExecute<U>();
            iExec.OnExecute += HandleProxyCallbackOnExecute;
            _proxyInterfaces.Add(iExec);
            return iExec.GetInterface();
        }

        public void RemoveProxyCallbackInterface<U>(string clientId) where U : class
        {
            var iExe = (InterfaceExecute<U>)_proxyInterfaces.FirstOrDefault(x => x.ClientId == clientId && x.InterfaceType == typeof(U));
            if (iExe != null)
            {
                _proxyInterfaces.Remove(iExe);
                iExe.OnExecute -= HandleProxyCallbackOnExecute;
            }
        }

        private List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
        private volatile int _nextFunctionId = 1;
        private void HandleProxyCallbackOnExecute(object sender, SerializedExecutionContext context)
        {
            var id = _nextFunctionId++;

            var functionDataPacket = new FunctionDataPacket
            {
                Id = id,
                ClassName = context.InterfaceFullName,
                FunctionName = context.UniqueMethodName,
                FunctionParameters = context.MethodParameters
            };
            
            var functionDataPacketSerialized = JsonConvert.SerializeObject(functionDataPacket);
            var content = new StringContent(functionDataPacketSerialized);
            var postResult =  _httpClient.PostAsync(_url, content).Result;
            
            if (postResult.IsSuccessStatusCode)
            {
                var resultContent = postResult.Content.ReadAsStringAsync().Result;
                var functionReturnData = JsonConvert.DeserializeObject<FunctionReturnDataPacket>(resultContent);
                if (functionReturnData.Exception != null)
                {
                    context.Exception = functionReturnData.Exception;
                }
                else
                {
                    context.ReturnValue = functionReturnData.ReturnValue;
                }
            }    
            else
            {
                context.Exception = new Exception($"Http error: {postResult.StatusCode}");
            }
        }
    }
}
