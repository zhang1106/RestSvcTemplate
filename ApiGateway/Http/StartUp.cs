using System.Net;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using AiDollar.Infrastructure.Logger;
using AiDollar.Infrastructure.Permissions;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Owin.Security.AesDataProtectorProvider;

namespace AiDollar.ApiGateway.Http
{
    public class StartUp
    {
        private readonly HttpServiceOptions _options;

        public StartUp(HttpServiceOptions options)
        {
            _options = options;
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            var logger = (ILogger) _options.DependencyResolver.GetService(typeof(ILogger));
            var permissionService =
                (IPermissionService) _options.DependencyResolver.GetService(typeof(IPermissionService));

            var http = new HttpConfiguration();
            http.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{key}",
                defaults: new {key = RouteParameter.Optional});

            http.SuppressDefaultHostAuthentication();
            http.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            http.Filters.Add(new AuthorizeFilter(logger, permissionService, _options.PermissionedApplications));

            http.Formatters.Clear();
            http.Formatters.Add(new JsonNetFormatter());
            http.DependencyResolver = _options.DependencyResolver;
            http.Services.Replace(typeof(IExceptionHandler), new LoggingExceptionHandler(logger));

            
            GlobalHost.Configuration.DisconnectTimeout = _options.SignalRConnectionTimeout;
            GlobalHost.Configuration.DefaultMessageBufferSize = _options.SignalRBufferSize;
            
            var serverOptions = new OAuthAuthorizationServerOptions 
            {
                TokenEndpointPath = new PathString("/oauth/token"),
                Provider = new NtlmOAuthAuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = _options.DefaultTokenExpiry,
                AllowInsecureHttp = true
            };

            var bearerOptions = new OAuthBearerAuthenticationOptions 
            {
                AuthenticationMode = AuthenticationMode.Active
            };

            if (_options.AllowTokenAsUrlParameter)
            {
                bearerOptions.Provider = new CustomOAuthBearerAuthenticationProvider("api_key");
            }

            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseOAuthAuthorizationServer(serverOptions);
            appBuilder.UseAesDataProtectorProvider();
            appBuilder.UseOAuthBearerAuthentication(bearerOptions);
            appBuilder.UseWebApi(http);

            var listener = (HttpListener) appBuilder.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemeSelectorDelegate = AuthenticationSchemeSelectorDelegate;

            SwaggerConfig.Register(http);
        }

        private AuthenticationSchemes AuthenticationSchemeSelectorDelegate(HttpListenerRequest httpRequest) 
        {
            if (httpRequest.Url.AbsolutePath.StartsWith("/api/"))
                return AuthenticationSchemes.Anonymous;

            return AuthenticationSchemes.Ntlm;
        }
    }
}
