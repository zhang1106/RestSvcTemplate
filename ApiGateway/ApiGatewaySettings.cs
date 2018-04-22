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
            get { return GetValue(() => PermissionedApplications, new[] { "PMUI", "OrderGateway" }); }
        }

        public TimeSpan PermissionRefreshInterval
        {
            get { return GetValue(() => PermissionRefreshInterval, TimeSpan.FromMinutes(30)); }
        }
        
        public HttpSettings Http { get; } = new HttpSettings();
      
        public string BamCoreLite
        {
            get { return GetGlobalValue(() => BamCoreLite); }
        }

        public string BrainConnectionString
        {
            get { return GetGlobalValue(() => BrainConnectionString); }
        }

        public string[] OmsServiceUris
        {
            get { return GetValue(() => OmsServiceUris); }
        }
    }
}
