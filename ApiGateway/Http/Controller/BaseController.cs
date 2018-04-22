using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using AiDollar.Infrastructure.Logger;
using AiDollar.Infrastructure.Permissions;

namespace AiDollar.ApiGateway.Http.Controller
{
    public abstract class BaseController : ApiController
    {
        private readonly IPermissionedEntityFilter _entityFilter;

        protected BaseController(IPermissionedEntityFilter entityFilter)
        {
            _entityFilter = entityFilter;
        }

        public ILogger Logger { get; set; }

        protected async Task<List<T>> FilterEntitiesAsync<T>(IEnumerable<T> items, Func<T, string> strategyAccessor)
        {
            var result = await _entityFilter.FilterAsync(items, RequestContext.Principal?.Identity, strategyAccessor);
            return result.ToList();
        }

    
        protected bool SpinUntil(Func<bool> predicate, TimeSpan timeout)
        {
            var start = DateTime.UtcNow;
            while (start + timeout > DateTime.UtcNow && !predicate())
            {
                Thread.Sleep(25);
            }

            return start + timeout <= DateTime.UtcNow;
        }

        protected string UserId => RequestContext.Principal?.Identity?.Name;
    }
}
