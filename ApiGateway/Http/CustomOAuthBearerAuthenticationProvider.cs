using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace AiDollar.ApiGateway.Http
{
    public class CustomOAuthBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {
        private readonly string _name;

        public CustomOAuthBearerAuthenticationProvider(string name) 
        {
            _name = name;
        }

        public override Task RequestToken(OAuthRequestTokenContext context) 
        {
            var value = context.Request.Query.Get(_name);
            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
                return Task.FromResult<object>(null);
            }

            return base.RequestToken(context);
        }
    }
}
