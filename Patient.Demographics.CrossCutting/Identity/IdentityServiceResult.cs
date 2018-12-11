using Microsoft.AspNet.Identity;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class IdentityServiceResult
    {
        public AspNetUser User { get; set; }
        public IdentityResult Result { get; set; }
    }
}