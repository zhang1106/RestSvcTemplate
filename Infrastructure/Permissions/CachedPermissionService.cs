using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Permissions
{
    public class CachedPermissionService : IPermissionService, IDisposable
    {
        private readonly IPermissionService _inner;
        private readonly TimeSpan _refreshInterval;

        private readonly Dictionary<CachedIdentity, ISet<string>> _strategiesByUser;
        private readonly Dictionary<KeyValuePair<CachedIdentity, string>, ISet<string>> _permissionsByUser;
        private readonly ReaderWriterLockSlim _accessLock;
        private readonly ManualResetEventSlim _exitEvent;

        public CachedPermissionService(IPermissionService inner, TimeSpan refreshInterval)
        {
            _inner = inner;
            _refreshInterval = refreshInterval;
            _accessLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _strategiesByUser = new Dictionary<CachedIdentity, ISet<string>>();
            _permissionsByUser = new Dictionary<KeyValuePair<CachedIdentity, string>, ISet<string>>();

            _exitEvent = new ManualResetEventSlim(false);
            ThreadPool.RegisterWaitForSingleObject(_exitEvent.WaitHandle, Refresh, null, _refreshInterval, true);
        }
        
        public async Task<ISet<string>> GetPermissionsAsync(IIdentity identity, string application)
        {
            var key = new KeyValuePair<CachedIdentity, string>(new CachedIdentity(identity), application);

            _accessLock.EnterReadLock();

            ISet<string> permissions;
            if (!_permissionsByUser.TryGetValue(key, out permissions))
            {
                _accessLock.ExitReadLock();

                permissions = await _inner.GetPermissionsAsync(identity, application);
                if (permissions.Count > 0)
                {
                    _accessLock.EnterWriteLock();
                    try
                    {
                        _permissionsByUser[key] = permissions;
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
                    }
                }
            }
            else
            {
                _accessLock.ExitReadLock();
            }

            return permissions;
        }

        public async Task<ISet<string>> GetStrategiesAsync(IIdentity identity)
        {
            var key = new CachedIdentity(identity);

            _accessLock.EnterReadLock();

            ISet<string> strategies;
            if (!_strategiesByUser.TryGetValue(key, out strategies))
            {
                _accessLock.ExitReadLock();

                strategies = await _inner.GetStrategiesAsync(identity);
                if (strategies.Count > 0)
                {
                    _accessLock.EnterWriteLock();
                    try
                    {
                        _strategiesByUser[key] = strategies;
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
                    }
                }
            }
            else
            {
                _accessLock.ExitReadLock();
            }

            return strategies;
        }

        public IIdentity GetIdentity(string user, string password)
        {
            return _inner.GetIdentity(user, password);
        }

        public IIdentity GetIdentity(string user)
        {
            return _inner.GetIdentity(user);
        }

        public void Dispose()
        {
            _exitEvent.Set();
        }

        private void Refresh(object state, bool timeout)
        {
            if (!timeout)
                return;

            try
            {
                _accessLock.EnterUpgradeableReadLock();
                try
                {
                    var strategyKeys = _strategiesByUser.Keys.ToArray();
                    var strategyTasks = new List<Task<ISet<string>>>();
                    foreach (var key in strategyKeys)
                    {
                        strategyTasks.Add(_inner.GetStrategiesAsync(key));
                    }

                    var permissionKeys = _permissionsByUser.Keys.ToArray();
                    var permissionTasks = new List<Task<ISet<string>>>();
                    foreach (var key in permissionKeys)
                    {
                        permissionTasks.Add(_inner.GetPermissionsAsync(key.Key, key.Value));
                    }

                    // ReSharper disable CoVariantArrayConversion
                    Task.WaitAll(strategyTasks.ToArray());
                    Task.WaitAll(permissionTasks.ToArray());
                    // ReSharper restore CoVariantArrayConversion

                    _accessLock.EnterWriteLock();
                    try
                    {
                        for (int i = 0; i < strategyKeys.Length; i++)
                        {
                            if (strategyTasks[i].Result.Count > 0)
                            {
                                _strategiesByUser[strategyKeys[i]] = strategyTasks[i].Result;
                            }
                        }

                        for (int i = 0; i < permissionKeys.Length; i++)
                        {
                            if (permissionTasks[i].Result.Count > 0)
                            {
                                _permissionsByUser[permissionKeys[i]] = permissionTasks[i].Result;
                            }
                        }
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
                    }
                }
                finally
                {
                    _accessLock.ExitUpgradeableReadLock();
                }
            }
            finally
            {
                ThreadPool.RegisterWaitForSingleObject(_exitEvent.WaitHandle, Refresh, null, _refreshInterval, true);
            }
        }
    }
}
