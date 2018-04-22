using System;
using System.Web.Http.Dependencies;
using Microsoft.Owin.Hosting;

namespace AiDollar.ApiGateway.Http
{
    public class HttpServiceOptions : StartOptions
    {
        public HttpServiceOptions(string baseUrl)
            : base(baseUrl)
        {
            
        }

        public string[] PermissionedApplications { get; set; }
        public IDependencyResolver DependencyResolver { get; set; }
        public TimeSpan SignalRConnectionTimeout { get; set; }
        public TimeSpan DefaultTokenExpiry { get; set; }
        public int SignalRBufferSize { get; set; }
        public bool DisableAuthorization { get; set; }
        public bool AllowTokenAsUrlParameter { get; set; }
    }
}
