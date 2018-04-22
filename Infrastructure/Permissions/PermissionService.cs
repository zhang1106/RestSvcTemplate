using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Threading.Tasks;
using AiDollar.Infrastructure.Logger;

namespace AiDollar.Infrastructure.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly Wcf.PermissionService.PermissionService _serviceClient;

        public PermissionService(Wcf.PermissionService.PermissionService serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public ILogger Logger { get; set; }
        
        public async Task<ISet<string>> GetPermissionsAsync(IIdentity identity, string application)
        {
            string userId = GetUserId(identity);
            var result = new HashSet<string>();
            if (userId != null)
            {
                try
                {
                    var infos = await _serviceClient.GetPermissionsAsync(userId, application);
                    foreach (var info in infos)
                    {
                        result.Add(info.PermissionPath);
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError($"Failed to request permissions for '{userId}'", ex);
                }
            }

            return result;
        }

        public async Task<ISet<string>> GetStrategiesAsync(IIdentity identity)
        {
            string userId = GetUserId(identity);
            var result = new HashSet<string>();
            if (userId != null)
            {
                try
                {
                    var infos = await _serviceClient.GetStrategiesAsync(userId);
                    foreach (var info in infos)
                    {
                        result.Add(info.StrategyCode);
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError($"Failed to request accessible strategies for '{userId}'", ex);
                }
            }

            return result;
        }

        public IIdentity GetIdentity(string user, string password)
        {
            using (var pc = new PrincipalContext(ContextType.Domain, "FOUNTAINHEAD"))
            {
                if (pc.ValidateCredentials(user, password))
                    return new CachedIdentity($"FOUNTAINHEAD\\{user}", true);
                else
                    return new CachedIdentity(user);
            }
        }

        public IIdentity GetIdentity(string user)
        {
            return new CachedIdentity($"FOUNTAINHEAD\\{user}", true);
        }

        private string GetUserId(IIdentity identity)
        {
            if (identity?.Name == null || !identity.Name.Contains(@"\"))
            {
                Logger?.LogWarning($"Cannot handle identity '{identity?.Name}'");
                return null;
            }

            return identity.Name.Remove(0,
                identity.Name.IndexOf(@"\", StringComparison.CurrentCultureIgnoreCase) + 1);
        }
    }
}
