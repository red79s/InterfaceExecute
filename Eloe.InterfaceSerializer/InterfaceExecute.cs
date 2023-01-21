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
        public EventHandler<SerializedExecutionContext> OnExecute;
        public EventHandler<EventInvokeData> OnEventInvoked;

        public Type InterfaceType { get; private set; }
        public List<MethodInf> Methods { get; }
        public string InterfaceName { get; private set; }
        public string InterfaceFullName { get; private set; }
        private readonly T _instance;
        private IParameterSerializer _parameterSerializer = new ParameterConvert();
        public string ClientId { get; set; }

        public InterfaceExecute(T instance)
        {
            InterfaceType = typeof(T);
            InterfaceName = InterfaceType.Name;
            InterfaceFullName = InterfaceType.FullName;
            _instance = instance;
            Methods = GetMethodAndTypeInfo(instance);
        }

        public InterfaceExecute()
        {
            InterfaceType = typeof(T);
            InterfaceName = InterfaceType.Name;
            InterfaceFullName = InterfaceType.FullName;
            Methods = GetMethodAndTypeInfo();
        }

        public T GetInterface()
        {
            ProxyGenerator generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget<T>(this);
        }

        public void Intercept(IInvocation invocation)
        {
            var method = GetMetodInfo(invocation.Method.Name);
            var executionContext = new SerializedExecutionContext
            {
                InterfaceFullName = InterfaceFullName,
                MethodName = method.Name,
                ExecutePath = method.Name,
                Payload = _parameterSerializer.Serialize(method.Parameters, invocation.Arguments.ToList()),
                HaveReturnValue = method.ReturnType != typeof(void),
                ReturnType = method.ReturnType,
                ClientId = ClientId
            };

            if (method.ReturnType == typeof(Task))
            {
                var taskCompleter = new TaskCompletionSource<object>();
                invocation.ReturnValue = taskCompleter.Task;
                Task.Run(() =>
                {
                    OnExecute?.Invoke(this, executionContext);

                    if (method.ReturnType != null)
                    {
                        var obj = _parameterSerializer.Deserialize("ReturnValue", method.ReturnType, executionContext.ReturnValue);
                        taskCompleter.SetResult(obj);
                    }
                    else
                    {
                        taskCompleter.SetResult(null);
                    }
                });
            }
            else
            {
                OnExecute?.Invoke(this, executionContext);

                if (method.ReturnType != null)
                {
                    invocation.ReturnValue =
                        _parameterSerializer.Deserialize("ReturnValue", method.ReturnType, executionContext.ReturnValue);
                }
            }
        }

        public bool HaveMethod(string method)
        {
            return GetMetodInfo(method) != null;
        }

        public MethodInf GetMetodInfo(string method)
        {
            return Methods.FirstOrDefault(x => x.Name == method);
        }

        public List<MethodPathInfo> GetMethodPaths()
        {
            var methodPathInfo = new List<MethodPathInfo>();
            foreach (var methodInf in Methods)
            {
                methodPathInfo.Add(new MethodPathInfo
                {
                    MethodPath = methodInf.Name,
                    HaveReturnValue = methodInf.ReturnType != typeof(void),
                    Executer = this
                });
            }

            return methodPathInfo;
        }

        public string Execute(string method, string parametersStr)
        {
            var methodInf = GetMetodInfo(method);
            if (methodInf == null)
                throw new ArgumentException($"Method: {method} does not exist", nameof(method));

            var parameterObjs = _parameterSerializer.Deserialize(methodInf.Parameters, parametersStr);
            var returnValue = methodInf.MethodInfo.Invoke(_instance, parameterObjs.ToArray());
            if (methodInf.ReturnType == null || returnValue == null)
                return null;

            return _parameterSerializer.Serialize("ReturnValue", methodInf.ReturnType, returnValue);
        }

        private List<MethodInf> GetMethodAndTypeInfo(T instance = null)
        {
            var methods = new List<MethodInf>();

            var type = typeof(T);

            foreach (var methodInfo in type.GetMethods())
            {
                var method = new MethodInf
                {
                    MethodInfo = methodInfo,
                    Name = methodInfo.Name,
                    Parameters = new List<ParameterInf>(),
                    ReturnType = methodInfo.ReturnType
                };

                if (methodInfo.ReturnType != null && methodInfo.ReturnType.GenericTypeArguments.Length > 0)
                {
                    method.ReturnTypeGenericType = methodInfo.ReturnType.GenericTypeArguments[0];
                }

                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    method.Parameters.Add(new ParameterInf
                    {
                        Name = parameterInfo.Name,
                        Type = parameterInfo.ParameterType
                    });
                }

                methods.Add(method);
            }

            if (instance != null)
            {
                foreach (var eventInfo in type.GetEvents())
                {
                    var genericMethod = new GenericMethod(paramaters => EventHandler(eventInfo, paramaters));

                    var eventDelegate = Dynamic.CoerceToDelegate(genericMethod, eventInfo.EventHandlerType);

                    eventInfo.AddEventHandler(instance, eventDelegate);
                }
            }
            return methods;
        }

        public delegate void GenericMethod(params object[] paramaters);

        public void EventHandler(EventInfo eventInfo, params object[] args)
        {
            var parameters = _parameterSerializer.Serialize("param", args[1].GetType(), args[1]);
            OnEventInvoked?.Invoke(this, new EventInvokeData
            {
                InterfaceName = InterfaceName,
                EventName = eventInfo.Name,
                Parameters = parameters
            });
        }
    }
}