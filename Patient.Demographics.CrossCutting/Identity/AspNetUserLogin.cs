using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class AspNetUserClaim : IdentityUserClaim<Guid> { }
    public class AspNetUserLogin : IdentityUserLogin<Guid> { }

    public class AspNetRole : IdentityRole<Guid, AspNetUserRole>
    {
        public AspNetRole() { }
        public AspNetRole(string name) { Name = name; }
    }
}