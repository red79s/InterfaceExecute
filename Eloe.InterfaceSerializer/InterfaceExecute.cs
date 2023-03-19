using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Dynamitey;

namespace Eloe.InterfaceSerializer
{
    public class InterfaceExecute<T> : IInterceptor, IInterfaceExecute where T : class
    {
        public delegate Task ExecuteDelegate(object sender, SerializedExecutionContext context);
        public ExecuteDelegate OnExecuteAsync;
        public EventHandler<SerializedExecutionContext> OnExecute;
        public Type InterfaceType { get; private set; }
        public List<MethodInf> Methods { get; }
        public string InterfaceName { get; private set; }
        public string InterfaceFullName { get; private set; }
        private readonly T _instance;
        private IParameterSerializer _parameterSerializer;
        public string ClientId { get; set; }

        public InterfaceExecute(T instance, IInterfaceSerializerOptions options = null)
        {
            SetOptions(options);
            InterfaceType = typeof(T);
            InterfaceName = InterfaceType.Name;
            InterfaceFullName = InterfaceType.FullName;
            _instance = instance;
            Methods = GetMethodAndTypeInfo(instance);
        }

        public InterfaceExecute(IInterfaceSerializerOptions options = null)
        {
            SetOptions(options);
            InterfaceType = typeof(T);
            InterfaceName = InterfaceType.Name;
            InterfaceFullName = InterfaceType.FullName;
            Methods = GetMethodAndTypeInfo();
        }

        private void SetOptions(IInterfaceSerializerOptions options)
        {
            if (options == null)
            {
                options = new InterfaceSerializerOptions { ParameterSerializer = new MsgPackParameterSerializer() };
            }

            _parameterSerializer = options.ParameterSerializer;
        }

        public T GetInterface()
        {
            ProxyGenerator generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget<T>(this);
        }

        public async void Intercept(IInvocation invocation)
        {
            var parameterTypes = new List<Type>();
            foreach (var parameter in invocation.Arguments)
            {
                parameterTypes.Add(parameter.GetType());
            }
            var method = GetMetodInfo(invocation.Method.Name, parameterTypes.ToArray());
            
            var executionContext = new SerializedExecutionContext
            {
                InterfaceFullName = InterfaceFullName,
                MethodName = method.Name,
                UniqueMethodName = method.UniqueName,
                MethodParameters = SerializeMethodParameters(method.Parameters, invocation.Arguments),
                HaveReturnValue = method.ReturnType != typeof(void),
                ReturnType = method.ReturnType,
                ClientId = ClientId
            };

            if (method.ReturnType == typeof(Task))
            {
                var taskCompleter = new TaskCompletionSource<object>();
                invocation.ReturnValue = taskCompleter.Task;
                
                await OnExecuteAsync?.Invoke(this, executionContext);

                if (executionContext.Exception != null)
                {
                    taskCompleter.SetException(executionContext.Exception);
                }
                else
                {
                    taskCompleter.SetResult(null);
                }
            }
            else if (method.ReturnType != null && method.ReturnType.BaseType == typeof(Task) && method.ReturnType.GenericTypeArguments.Length > 0)
            {
                var taskCompletionType = typeof(TaskCompletionSource<>);
                var taskCompletionGenericType = taskCompletionType.MakeGenericType(method.ReturnType.GenericTypeArguments[0]);
                var taskCompleterInstance = Activator.CreateInstance(taskCompletionGenericType);

                var taskProperty = taskCompletionGenericType.GetProperties().FirstOrDefault(x => x.Name == "Task");
                var setResult = taskCompletionGenericType.GetMethod("SetResult");
                var setException = taskCompletionGenericType.GetMethod("SetException", new Type[] { typeof(Exception)});

                invocation.ReturnValue = taskProperty.GetValue(taskCompleterInstance, null);
                
                Console.WriteLine($"OnExecute: {executionContext.UniqueMethodName}");
                await OnExecuteAsync?.Invoke(this, executionContext);
                Console.WriteLine($"OnExecute returned: {executionContext.UniqueMethodName}");
                if (executionContext.Exception != null)
                {
                    setException.Invoke(taskCompleterInstance, new object[] { executionContext.Exception });
                }
                else if (method.ReturnType != null && method.ReturnType != typeof(void))
                {
                    var returnObj = _parameterSerializer.Deserialize("ReturnValue", method.ReturnType.GenericTypeArguments[0], executionContext.ReturnValue);
                    setResult.Invoke(taskCompleterInstance, new object[] { returnObj });
                }
            }
            else
            {
                OnExecute?.Invoke(this, executionContext);

                if (executionContext.Exception != null)
                {
                    throw new Exception($"Unhandled exception in {executionContext.InterfaceFullName}.{executionContext.MethodName}", executionContext.Exception);
                }

                if (method.ReturnType != null && method.ReturnType != typeof(void))
                {
                    invocation.ReturnValue =
                        _parameterSerializer.Deserialize("ReturnValue", method.ReturnType, executionContext.ReturnValue);
                }
            }
        }

        private List<byte[]> SerializeMethodParameters(List<ParameterInf> parametersInfo, object[] arguments)
        {
            var parameters = new List<byte[]>();
            for (int i = 0; i < parametersInfo.Count; i++)
            {
                parameters.Add(_parameterSerializer.Serialize(parametersInfo[i].Name, parametersInfo[i].Type, arguments[i]));
            }
            return parameters;
        }

        public MethodInf GetMetodInfo(string uniqueMethodName)
        {
            return Methods.FirstOrDefault(x => x.UniqueName == uniqueMethodName);
        }

        public MethodInf GetMetodInfo(string methodName, Type[] parameterTypes)
        {
            var methods = Methods.Where(x => x.Name == methodName).ToList();
            if (methods.Count == 0)
            {
                throw new Exception($"Invalid method name, interface does not contain: {methodName}");
            }
            if (methods.Count == 1)
            {
                return methods[0];
            }

            foreach (var method in methods)
            {
                if (method.Parameters.Count == parameterTypes.Length)
                {
                    var isMatch = true;
                    for (int i = 0; i < method.Parameters.Count; i++)
                    {
                        if (method.Parameters[i].Type != parameterTypes[i])
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        return method;
                    }
                }
            }

            throw new Exception($"Invalid method name and parameters, there is no match for method: {methodName} with arguments: {string.Join(", ", parameterTypes.Select(x => x.Name))}");
        }

        public byte[] Execute(string uniqueMethodName, List<byte[]> serializedParameters, string jwtToken = null, IAuthorizeHandler authorizeHandler = null)
        {
            var methodInf = GetMetodInfo(uniqueMethodName);
            if (methodInf == null)
                throw new ArgumentException($"Method: {uniqueMethodName} does not exist", nameof(uniqueMethodName));

            if (methodInf.Authorize.RequireAuthorization)
            {
                if (authorizeHandler == null)
                {
                    throw new Exception("Method requires authorization, but not AuthorizationHandler is provided");
                }

                authorizeHandler.Authorize(jwtToken, methodInf.Authorize.Roles);
            }

            var parameterObjs = DeserializeParameters(methodInf.Parameters, serializedParameters);
            var returnValue = methodInf.MethodInfo.Invoke(_instance, parameterObjs.ToArray());
            if (methodInf.ReturnType == null || returnValue == null)
                return null;

            if (methodInf.ReturnType == typeof(Task))
            {
                var waitMethod = methodInf.ReturnType.GetMethod("Wait", new Type[0]);
                waitMethod.Invoke(returnValue, new object[0]);
                return null;
            }
            else if (methodInf.ReturnType != null && methodInf.ReturnType.BaseType == typeof(Task) && methodInf.ReturnType.GenericTypeArguments.Length > 0)
            {
                var resultProperty = methodInf.ReturnType.GetProperty("Result");
                var returnValueObj = resultProperty.GetValue(returnValue, null);
                return _parameterSerializer.Serialize("ReturnValue", methodInf.ReturnType.GenericTypeArguments[0], returnValueObj);
            }
            return _parameterSerializer.Serialize("ReturnValue", methodInf.ReturnType, returnValue);
        }

        private object[] DeserializeParameters(List<ParameterInf> parametersInfo, List<byte[]> serializedParameters)
        {
            var parameters = new object[serializedParameters.Count];
            for (int i = 0; i < serializedParameters.Count; i++)
            {
                parameters[i] = _parameterSerializer.Deserialize(parametersInfo[i].Name, parametersInfo[i].Type, serializedParameters[i]);
            }
            return parameters;
        }

        private List<MethodInf> GetMethodAndTypeInfo(T instance = null)
        {
            var methodNameOverloads = new Dictionary<string, int>();
            var methods = new List<MethodInf>();

            var type = typeof(T);
            if (instance != null)
                type = instance.GetType();

            foreach (var methodInfo in type.GetMethods())
            {
                var method = new MethodInf
                {
                    MethodInfo = methodInfo,
                    Name = methodInfo.Name,
                    Parameters = new List<ParameterInf>(),
                    ReturnType = methodInfo.ReturnType
                };

                if (methodNameOverloads.ContainsKey(method.Name))
                {
                    methodNameOverloads[method.Name]++;
                }
                else
                {
                    methodNameOverloads.Add(method.Name, 0);
                }
                method.UniqueName = $"{method.Name}:{methodNameOverloads[method.Name]}";

                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    method.Parameters.Add(new ParameterInf
                    {
                        Name = parameterInfo.Name,
                        Type = parameterInfo.ParameterType
                    });
                }

                var authAttribute = methodInfo.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "AuthorizeAttribute");
                method.Authorize = new AuthorizeInf(authAttribute);

                methods.Add(method);
            }

            return methods;
        }
    }
}