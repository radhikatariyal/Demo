using System.Collections.Generic;
using System.Security.Principal;

namespace Patient.Demographics.Web
{
    public class Principal : IPrincipal
    {
        private readonly Identity _identity;
        private readonly List<string> _roles = new List<string>();

        public Principal(Identity identity, string[] roles)
        {
            _roles.AddRange(roles);
            _identity = identity;
        }

        public IIdentity Identity => _identity;

        public bool IsInRole(string role)
        {
            return _roles.Contains(role);
        }
    }
}