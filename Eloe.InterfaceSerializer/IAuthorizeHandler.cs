using System.Collections.Generic;

namespace Eloe.InterfaceSerializer
{
    public interface IAuthorizeHandler
    {
        void Authorize(string jwtToken);
        void Authorize(string jwtToken, List<string> roles);
    }
}
