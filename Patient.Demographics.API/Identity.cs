using System.Security.Principal;

namespace Patient.Demographics.Web
{
    public class Identity : IIdentity
    {
        public string AuthenticationType => "Custom";

        public bool IsAuthenticated => true;

        public string Name => Username;

        public string Username { get; set; }
    }
}