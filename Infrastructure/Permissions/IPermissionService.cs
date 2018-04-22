using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Permissions
{
    public interface IPermissionService
    {
        Task<ISet<string>> GetPermissionsAsync(IIdentity identity, string application);
        Task<ISet<string>> GetStrategiesAsync(IIdentity identity);
        IIdentity GetIdentity(string user, string password);
        IIdentity GetIdentity(string user);
    }
}
