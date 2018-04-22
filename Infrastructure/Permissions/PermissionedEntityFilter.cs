using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Permissions
{
    public class PermissionedEntityFilter : IPermissionedEntityFilter
    {
        private readonly IPermissionService _permissionService;

        public PermissionedEntityFilter(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IEnumerable<T>> FilterAsync<T>(IEnumerable<T> items, IIdentity user, Func<T, string> strategyAccessor)
        {
            var strategies = await _permissionService.GetStrategiesAsync(user);

            var result = new List<T>();
            foreach (var item in items)
            {
                var strategy = strategyAccessor(item);
                if (strategies.Contains(strategy))
                {
                    result.Add(item);
                }
                else if (strategy.IndexOf('-') != strategy.LastIndexOf('-'))
                {
                    strategy = strategy.Substring(0, strategy.LastIndexOf('-'));
                    if (strategies.Contains(strategy))
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }
}