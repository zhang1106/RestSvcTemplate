using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Permissions
{
    public class NoAuthPermissionedEntityFilter : IPermissionedEntityFilter
    {
        public Task<IEnumerable<T>> FilterAsync<T>(IEnumerable<T> items, IIdentity user, Func<T, string> strategyAccessor)
        {
            return Task.FromResult(items);
        }
    }
}
