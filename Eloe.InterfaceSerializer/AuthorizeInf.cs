using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eloe.InterfaceSerializer
{
    public class AuthorizeInf
    {
        public bool RequireAuthorization { get; set; }
        public List<string> Roles { get; set; }
        
        public AuthorizeInf()
        {
            RequireAuthorization = false;
            Roles = new List<string>();
        }

        public AuthorizeInf(CustomAttributeData authAttribute)
            : this()
        {
            if (authAttribute == null || authAttribute.AttributeType.Name != "AuthorizeAttribute")
            {
                return;
            }

            RequireAuthorization = true;

            var customAttributeNamedArgument = authAttribute.NamedArguments.FirstOrDefault(x => x.MemberName == "Roles");
            if (customAttributeNamedArgument != null && customAttributeNamedArgument.TypedValue.Value != null)
            {
                var rolesStr = customAttributeNamedArgument.TypedValue.Value.ToString();
                var rolesArray = rolesStr.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var role in rolesArray )
                {
                    if (string.IsNullOrWhiteSpace(role)) 
                        continue;
                    Roles.Add(role.Trim());
                }
            }
        }
    }
}
