using System;
using System.Collections.Generic;
using System.Linq;

namespace AiDollar.ApiGateway.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiredPermissionsAttribute : Attribute
    {
        public RequiredPermissionsAttribute(string permissions)
        {
            Permissions = permissions
                .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();
        }

        public IReadOnlyList<string> Permissions { get; set; }
    }
}
