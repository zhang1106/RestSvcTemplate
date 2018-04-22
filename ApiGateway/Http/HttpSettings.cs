using System;
using AiDollar.Infrastructure.Configuration;

namespace AiDollar.ApiGateway.Http
{
    public class HttpSettings : AppSettingsBase
    {
        public HttpSettings() : base ("ApiGateway.Http.")
        {
            
        }

        public string BaseUrl
        {
            get { return GetValue(() => BaseUrl, "http://localhost:9777"); }
        }

        public TimeSpan DefaultTokenExpiry
        {
            get { return GetValue(() => DefaultTokenExpiry, TimeSpan.FromDays(1)); }
        }

        public bool AllowTokenAsUrlParameter
        {
            get { return GetValue(() => AllowTokenAsUrlParameter, true); }
        }
    }
}
