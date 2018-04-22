using System;
using AiDollar.Infrastructure.Hosting;
using Microsoft.Owin.Hosting;

namespace AiDollar.ApiGateway.Http
{
    public class HttpService : IService
    {
        private readonly HttpServiceOptions _options;
        private IDisposable _web;

        public HttpService(HttpServiceOptions options)
        {
            _options = options;
        }

        public string Name => "HTTP";

        public void Start()
        {
            _web = WebApp.Start(_options, 
                app => new StartUp(_options).Configuration(app));
        }

        public void Stop()
        {
            _web.Dispose();
        }
    }
}
