using Patient.Demographics.CrossCutting.Identity;
using Microsoft.AspNet.Identity;
using System;

namespace Patient.Demographics.Data.Identity
{
    public class AspNetUserManager : UserManager<AspNetUser, Guid>
    {
        public AspNetUserManager(IUserStore<AspNetUser, Guid> store)
            : base(store)
        {
            UserValidator = new UserValidator<AspNetUser, Guid>(this)
            {
                AllowOnlyAlphanumericUserNames = false
            };
        }
    }
}