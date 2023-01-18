using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eloe.InterfaceSerializer
{
    public class MethodInf
    {
        public string Name { get; set; }
        public List<ParameterInf> Parameters { get; set; }
        public Type ReturnType { get; set; }
        public Type ReturnTypeGenericType { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }
}