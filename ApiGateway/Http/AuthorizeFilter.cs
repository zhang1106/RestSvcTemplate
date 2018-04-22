using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using AiDollar.Infrastructure.Logger;
using AiDollar.Infrastructure.Permissions;

namespace AiDollar.ApiGateway.Http
{
    public class AuthorizeFilter : AuthorizeAttribute
    {
        private readonly ILogger _logger;
        private readonly IPermissionService _permissionService;
        private readonly string[] _applications;

        public AuthorizeFilter(ILogger logger, IPermissionService permissionService, string[] applications)
        {
            _logger = logger;
            _permissionService = permissionService;
            _applications = applications;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (!base.IsAuthorized(actionContext))
                return false;

            var attr = actionContext.ActionDescriptor?
                .GetCustomAttributes<RequiredPermissionsAttribute>().FirstOrDefault();
            if (attr == null) return true;

            var required = attr.Permissions;
            if (!required.Any())
                return true;

            var tasks = _applications.Select(app => _permissionService.GetPermissionsAsync(
                actionContext.RequestContext.Principal.Identity, app).Result).ToList();

            var allowed = new HashSet<string>();
            foreach (var t in tasks)
            {
                allowed.UnionWith(t);
            }

            return required.Any(p => allowed.Contains(p));
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (!actionContext.RequestContext.Principal?.Identity.IsAuthenticated ?? false)
            {
                _logger.LogWarning($"Detected unauthenticated request to '{actionContext.Request.RequestUri}'");
            }
            else
            {
                _logger.LogWarning($"Detected unauthorized request to '{actionContext.Request.RequestUri}' by '{actionContext.RequestContext.Principal?.Identity.Name}'");
            }

            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}
