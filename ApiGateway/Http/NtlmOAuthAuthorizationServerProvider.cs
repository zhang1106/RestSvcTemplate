using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace AiDollar.ApiGateway.Http
{
    public class NtlmOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) 
        {
            if (!(context.Request.User?.Identity?.IsAuthenticated ?? false))
            {
                context.SetError("invalid_grant");
                context.Rejected();
            }
            else
            {
                TimeSpan? expiry = null;
                if (context.Parameters["expiry"] != null)
                {
                    TimeSpan ts;
                    if (TimeSpan.TryParse(context.Parameters["expiry"], out ts))
                    {
                        expiry = ts;
                    }
                }

                if (expiry != null)
                {
                    context.Options.AccessTokenExpireTimeSpan = expiry.Value;
                }

                context.Validated();
            }

            return Task.FromResult(0);
        }

        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context) 
        {
            var original = (ClaimsIdentity)context.Request.User.Identity;
            var identity = new ClaimsIdentity(original.AuthenticationType);
            identity.AddClaim(new Claim(original.NameClaimType, original.Name));
            context.Validated(identity);
            return Task.FromResult(0);
        }
    }
}
