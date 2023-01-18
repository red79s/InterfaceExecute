using System;
using System.Collections.Generic;
using System.Text;

namespace Eloe.InterfaceSerializer
{
    public interface IParameterSerializer
    {
        string Serialize(string name, Type type, object value);
        string Serialize(List<ParameterInf> parameterInfs, List<object> values);
        object Deserialize(string name, Type type, string parameterStr);
        List<object> Deserialize(List<ParameterInf> parameterInfs, string parametersStr);
    }
}
