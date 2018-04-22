using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using AiDollar.Infrastructure.Permissions;

namespace AiDollar.ApiGateway.Http.Controller
{
    public class PermissionController : ApiController
    {
        private readonly IPermissionService _permissionService;
        private readonly string[] _applications;

        public PermissionController(IPermissionService permissionService, string[] applications)
        {
            _permissionService = permissionService;
            _applications = applications;
        }

        public async Task<List<string>> GetPermissions()
        {
            var tasks = _applications.Select(app => _permissionService.GetPermissionsAsync(Identity, app))
                .ToList();
            await Task.WhenAll(tasks);
            var permissions = tasks.SelectMany(t => t.Result).Distinct();
            return permissions.OrderBy(p => p).ToList();
        }

        public async Task<List<string>> Get(string app)
        {
            var permissions = await _permissionService.GetPermissionsAsync(Identity, app);
            return permissions.OrderBy(p => p).ToList();
        }

        public async Task<List<string>> GetStrategies()
        {
            var strategies = await _permissionService.GetStrategiesAsync(Identity);
            return strategies.OrderBy(s => s).ToList();
        }

        [AllowAnonymous]
        public HttpResponseMessage GetToken()
        {
            var assembly = typeof(PermissionController).Assembly;
            var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("Token.html"));

            var stream = assembly.GetManifestResourceStream(resourceName);
            var response = new HttpResponseMessage {Content = new StreamContent(stream)};
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        protected IIdentity Identity => RequestContext.Principal?.Identity;
    }
}
