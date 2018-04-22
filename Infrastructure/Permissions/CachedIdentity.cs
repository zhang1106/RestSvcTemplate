using System;
using System.Security.Principal;

namespace AiDollar.Infrastructure.Permissions
{
    public class CachedIdentity : IIdentity, IEquatable<CachedIdentity>
    {
        public bool Equals(CachedIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CachedIdentity)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(CachedIdentity left, CachedIdentity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CachedIdentity left, CachedIdentity right)
        {
            return !Equals(left, right);
        }

        public CachedIdentity(IIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
            Name = identity.Name;
            AuthenticationType = identity.AuthenticationType;
            IsAuthenticated = identity.IsAuthenticated;
        }
        public CachedIdentity(string name, bool isAuth = false)
        {
            Name = name;
            AuthenticationType = "";
            IsAuthenticated = true;
        }

        public string Name { get; }
        public string AuthenticationType { get; }
        public bool IsAuthenticated { get; }
    }
}
