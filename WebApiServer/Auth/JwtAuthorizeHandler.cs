using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Eloe.InterfaceSerializer;

namespace Eloe.WebApiServer.Auth
{
    public class JwtAuthorizeHandler : IAuthorizeHandler
    {
        private readonly string _authServer;
        private OpenIdConnectConfiguration? _openIdConfig = null;
        private List<string> _defaultValidAudiences = new List<string>();

        public JwtAuthorizeHandler(string authServer, List<string>? validAudiences = null)
        {
            _authServer = authServer;
            if (validAudiences != null)
            {
                _defaultValidAudiences.AddRange(validAudiences);
            }
        }

        public async Task GetAuthServerConfiguration()
        {
            var openIdConfigurationEndpoint = $"{_authServer}.well-known/openid-configuration";
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(openIdConfigurationEndpoint, new OpenIdConnectConfigurationRetriever());
            _openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
        }

        public void Authorize(string JwtToken)
        {
            if (_openIdConfig == null)
            {
                throw new UnauthorizedAccessException("Authorizer is not initialized");
            }

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _authServer,
                ValidAudiences = _defaultValidAudiences.ToArray(),
                IssuerSigningKeys = _openIdConfig.SigningKeys
            };

            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(JwtToken, validationParameters, out validatedToken);
        }

        public void Authorize(string JwtToken, List<string> roles)
        {
            if (_openIdConfig == null)
            {
                throw new UnauthorizedAccessException("Authorizer is not initialized");
            }

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _authServer,
                ValidAudiences = _defaultValidAudiences.ToArray(),
                IssuerSigningKeys = _openIdConfig.SigningKeys
            };

            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var claimsPrincipal = handler.ValidateToken(JwtToken, validationParameters, out validatedToken);

            var authorizedRoles = claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            foreach (var r in roles)
            {
                if (authorizedRoles.FirstOrDefault(x => x.Value.Equals(r, StringComparison.CurrentCultureIgnoreCase)) == null)
                {
                    throw new UnauthorizedAccessException("User is not authorized");
                }
            }
        }
    }
}
