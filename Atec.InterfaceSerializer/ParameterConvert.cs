using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Atec.InterfaceSerializer
{
    public class ParameterConvert : IParameterSerializer
    {
        public string Serialize(string name, Type type, object value)
        {
            string valueStr;
            if (type.IsValueType)
            {
                valueStr = value.ToString();
                return $"{{\"{name}\": \"{valueStr}\"}}";
            }
            else
            {
                valueStr = JsonConvert.SerializeObject(value);
                return $"{{\"{name}\": {valueStr}}}";
            }
        }

        public string Serialize(List<ParameterInf> parameterInfs, List<object> values)
        {
            var exo = new System.Dynamic.ExpandoObject();
            for (int i = 0; i < parameterInfs.Count; i++)
            {
                ((IDictionary<String, Object>)exo).Add(parameterInfs[i].Name, values[i]);
            }
            var str = JsonConvert.SerializeObject(exo);
            return str;
        }

        //public static string Serialize(List<ParameterInf> parameterInfs, List<object> values)
        //{
        //    string serializedParameters = "{";
        //    for (var i = 0; i < parameterInfs.Count; i++)
        //    {
        //        if (i > 0)
        //        {
        //            serializedParameters += ",";
        //        }

        //        string valueStr;
        //        if (parameterInfs[i].Type.IsValueType)
        //        {
        //            valueStr = values[i].ToString();
        //            serializedParameters += $"\"{parameterInfs[i].Name}\": \"{valueStr}\"";
        //        }
        //        else
        //        {
        //            valueStr = JsonConvert.SerializeObject(values[i]);
        //            serializedParameters += $"\"{parameterInfs[i].Name}\": {valueStr}";
        //        }
        //    }

        //    serializedParameters += "}";
        //    return serializedParameters;
        //}

        public object Deserialize(string name, Type type, string parameterStr)
        {
            if (string.IsNullOrEmpty(parameterStr))
                return null;

            var jParams = JObject.Parse(parameterStr);
            if (type.IsValueType)
            {
                var jValue = jParams[name] as JValue;
                if (jValue == null)
                {
                    return null;
                }
                var val = Convert.ChangeType(jValue.Value.ToString(), type);
                return val;
            }
            else
            {
                var jObj = jParams[name];
                
                if (type == typeof(string))
                {
                    return jObj.ToString();
                }
                else
                {
                    return JsonConvert.DeserializeObject(jObj.ToString(), type);
                }
            }
        }

        public List<object> Deserialize(List<ParameterInf> parameterInfs, string parametersStr)
        {
            List<object> paramObjects = new List<object>();
            if (parameterInfs.Count == 0)
                return new List<object>();
            
            var jParams = JObject.Parse(parametersStr);
            foreach (var paramInfo in parameterInfs)
            {
                if (paramInfo.Type.IsValueType)
                {
                    var jValue = jParams[paramInfo.Name] as JValue;
                    if (jValue == null)
                    {
                        paramObjects.Add(null);
                        continue;
                    }
                    var val = Convert.ChangeType(jValue.Value.ToString(), paramInfo.Type);
                    paramObjects.Add(val);
                }
                else
                {
                    var jObj = jParams[paramInfo.Name];
                    object val;
                    if (paramInfo.Type == typeof(string))
                    {
                        val = jObj.ToString();
                    }
                    else
                    {
                        val = JsonConvert.DeserializeObject(jObj.ToString(), paramInfo.Type);
                    }
                    paramObjects.Add(val);
                }
            }

            return paramObjects;
        }
    }
}