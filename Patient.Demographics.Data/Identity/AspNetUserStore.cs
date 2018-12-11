using System;
using Patient.Demographics.CrossCutting.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Patient.Demographics.Data.Identity
{
    public class AspNetUserStore : UserStore<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim>
    {
        public AspNetUserStore(IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim> context)
            : base(context)
        {
        }
    }
}