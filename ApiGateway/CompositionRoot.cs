using System;
using System.Collections.Generic;
using System.Linq;
using AiDollar.Infrastructure.Hosting;
using AiDollar.ApiGateway.Http;
using AiDollar.Infrastructure.Logger;
using AiDollar.Infrastructure.Permissions;
using AiDollar.Infrastructure.Threading;
using AiDollar.Infrastructure.Wcf.PermissionService;
using StructureMap;
using System.Web.Http.Dependencies;

namespace AiDollar.ApiGateway
{
    public class CompositionRoot : ICompositionRoot<IService>
    {
        private readonly IContainer _container;
        private readonly ApiGatewaySettings _settings;

        protected static CompositionRoot SvcComposite;

        public static CompositionRoot CompositeRootInstanace()
        {
            return SvcComposite ?? (SvcComposite = new CompositionRoot());
        }

        public CompositionRoot()
        {
            _settings = new ApiGatewaySettings();
            _container = new Container(
                    config =>
                    {
                        ConfigureLogger(config, _settings);
                        ConfigureRest(config, _settings);
                    }
                );

            _container.Inject(typeof(IDependencyResolver), new StructureMapDependencyResolver(_container));
        }

        protected virtual void ConfigureLogger(ConfigurationExpression config, ApiGatewaySettings settings)
        {
            var logger = BackgroundWorkerFactory.Logger ?? new Log4NetLogger(settings.LogPath, settings.LogFilename, settings.LogArchivePath);
            config.For<ILogger>().Use(logger);
            config.Policies.FillAllPropertiesOfType<ILogger>();
            BackgroundWorkerFactory.Logger = logger;
        }
        private void ConfigureRest(ConfigurationExpression config, ApiGatewaySettings settings)
        {
            var httpConfig = config.ForConcreteType<HttpServiceOptions>().Configure
                .Ctor<string>().Is(settings.Http.BaseUrl)
                .Setter(m => m.PermissionedApplications).Is(settings.PermissionedApplications)
                .Setter(m => m.DependencyResolver).IsTheDefault()
                .Setter(m => m.DisableAuthorization).Is(settings.DisableAuthorization)
                .Setter(m => m.DefaultTokenExpiry).Is(settings.Http.DefaultTokenExpiry)
                .Setter(m => m.AllowTokenAsUrlParameter).Is(settings.Http.AllowTokenAsUrlParameter);

            config.For<IService>().Add<HttpService>()
                .Named(nameof(HttpService))
                .Ctor<HttpServiceOptions>().Is(httpConfig);
            
            config
               .ForConcreteType<ApiGatewayService>().Configure
               .Ctor<IService>("httpService").Is(ctx => ctx.GetInstance<IService>(nameof(HttpService)));

            //controllers
            config.ForConcreteType<Http.Controller.TestController>();

        }

        protected virtual void ConfigureAuthorization(ConfigurationExpression config, ApiGatewaySettings settings, TimeSpan permissionRefresh = default(TimeSpan))
        {
            config.For<IPermissionedEntityFilter>().Use<PermissionedEntityFilter>().Singleton();

            config
                .For<IPermissionService>()
                .Use<CachedPermissionService>()
                .Ctor<TimeSpan>().Is(permissionRefresh == default(TimeSpan) ? TimeSpan.FromMinutes(30) : permissionRefresh)
                .Ctor<IPermissionService>().Is<Infrastructure.Permissions.PermissionService>(cfg => cfg
                    .Ctor<Infrastructure.Wcf.PermissionService.PermissionService>().Is("service",
                        () => new PermissionServiceClient("TcpPermissionEndpoint")));

            if (settings.DisableAuthorization)
            {
                config.For<IPermissionedEntityFilter>()
                    .Use<NoAuthPermissionedEntityFilter>()
                    .Singleton();
            }
            else
            {
                config.For<IPermissionedEntityFilter>()
                    .Use<PermissionedEntityFilter>()
                    .Singleton();
            }
        }
        public IService Initialize()
        {
            var svc = _container.GetInstance<IService>();
            return svc;
        }

        public void Dispose()
        { }

        private class StructureMapDependencyResolver : IDependencyResolver
        {
            private readonly IContainer _container;

            public StructureMapDependencyResolver(IContainer container)
            {
                _container = container;
            }

            public void Dispose()
            {
                // nothing
            }

            public object GetService(Type serviceType)
            {
                return   _container.TryGetInstance(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return _container.GetAllInstances(serviceType).Cast<object>();
            }

            public IDependencyScope BeginScope()
            {
                return new StructureMapDependencyScope(_container.GetNestedContainer());
            }

            private class StructureMapDependencyScope : IDependencyScope
            {
                private readonly IContainer _container;

                public StructureMapDependencyScope(IContainer container)
                {
                    _container = container;
                }

                public void Dispose()
                {
                    _container.Dispose();
                }

                public object GetService(Type serviceType)
                {
                    return   _container.GetInstance(serviceType);
                }

                public IEnumerable<object> GetServices(Type serviceType)
                {
                    return _container.GetAllInstances(serviceType).Cast<object>();
                }
            }
        }
    }
}
