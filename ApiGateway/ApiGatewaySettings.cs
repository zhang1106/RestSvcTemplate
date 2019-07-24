using System;
using AiDollar.ApiGateway.Http;
using AiDollar.Infrastructure.Configuration;

namespace AiDollar.ApiGateway
{
    public class ApiGatewaySettings : SettingsBase
    {
        public ApiGatewaySettings(): base("ApiGateway.")
        {
            
        }

        public bool DisableAuthorization
        {
            get { return GetValue(() => DisableAuthorization, GetGlobalValue(() => DisableAuthorization)); }
        }

        public string[] PermissionedApplications
        {
            get { return GetValue(() => PermissionedApplications, new[] { "PMUI"}); }
        }

        public TimeSpan PermissionRefreshInterval
        {
            get { return GetValue(() => PermissionRefreshInterval, TimeSpan.FromMinutes(30)); }
        }

        public string Environment
        {
            get {return GetGlobalValue(() => Environment); }
        }
        
        public HttpSettings Http { get; } = new HttpSettings();
      
        public string AiDollarCoreLite
        {
            get { return GetGlobalValue(() => AiDollarCoreLite); }
        }
    }
}
