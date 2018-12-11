using System;
using Patient.Demographics.CrossCutting.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Patient.Demographics.Data.Identity
{
    public interface IApplicationUserManagerFactory
    {
        AspNetUserManager CreateApplicationUserManager(IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim> dbContext);
    }

    public class AspNetUserManagerFactory : IApplicationUserManagerFactory
    {
        public AspNetUserManager CreateApplicationUserManager(IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim> dbContext)
        {
            return new AspNetUserManager(new AspNetUserStore(dbContext));
        }
    }
}