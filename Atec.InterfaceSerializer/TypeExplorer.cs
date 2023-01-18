using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atec.InterfaceSerializer
{
    public class TypeExplorer<T> where T : class
    {
        private T _instance;
        private MethodInfo[] _methods;

        public TypeExplorer(T instance)
        {
            _instance = instance;
            var type = typeof(T);
            _methods = type.GetMethods();
        }
       
        public void Execute(string methodName, string parameters)
        {
            foreach (var methodInfo in _methods)
            {
                if (methodInfo.Name == methodName)
                {
                    var p = methodInfo.GetParameters();
                    if (p.Length == 0)
                    {
                        var res = methodInfo.Invoke(_instance, null);
                    }
                    else if (p.Length == 1)
                    {
                        var t = p[0].ParameterType;
                       
                        if (!t.IsValueType)
                        {
                            var par = JsonConvert.DeserializeObject(parameters, t);
                            var res = methodInfo.Invoke(_instance, new object[] { par });
                        }

                    }
                    
                }
            }
        }

        public List<object> GetParameters(string methodName, string parameters)
        {
            var parameterInfo = GetParameterInfo(methodName);
            List<object> paramObjects = new List<object>();
            if (parameterInfo.Length == 0)
                return null;


            if (parameterInfo.Length == 1)
            {
                if (parameterInfo[0].ParameterType.IsValueType)
                {
                    var val = GetValueType(parameterInfo[0], parameters);
                    paramObjects.Add(val);
                }
                else
                {
                    var val = JsonConvert.DeserializeObject(parameters, parameterInfo[0].ParameterType);
                    paramObjects.Add(val);
                }
            }
            else
            {
                var jParams = JObject.Parse(parameters);
                foreach (var paramInfo in parameterInfo)
                {
                    if (paramInfo.ParameterType.IsValueType)
                    {
                        var jValue = jParams[paramInfo.Name] as JValue;
                        if (jValue == null)
                        {
                            paramObjects.Add(null);
                            continue;
                        }
                        var val = Convert.ChangeType(jValue.Value.ToString(), paramInfo.ParameterType);
                        paramObjects.Add(val);
                    }
                    else
                    {
                        var jObj = jParams[paramInfo.Name];
                        var val = JsonConvert.DeserializeObject(jObj.ToString(), paramInfo.ParameterType);
                        paramObjects.Add(val);
                    }
                }
            }

            return paramObjects;
        }

        public object GetValueType(ParameterInfo pInfo, string parametersString)
        {
            var jObj = JObject.Parse(parametersString);
            var val = jObj[pInfo.Name] as JValue;
            if (val == null)
                return null;

            return Convert.ChangeType(val.Value.ToString(), pInfo.ParameterType);
        }

        private ParameterInfo[] GetParameterInfo(string methodName)
        {
            foreach (var methodInfo in _methods)
            {
                if (methodInfo.Name == methodName)
                {
                    return methodInfo.GetParameters();
                }
            }

            return null;
        }
    }
}
