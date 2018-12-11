using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class AspNetUser : IdentityUser<Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim>
    {
        [Required]
        public DateTime JoinDate { get; set; }

        [MaxLength(256)]
        public string SecurityQuestion { get; set; }

        [MaxLength(128)]
        public string SecurityAnswer { get; set; }

        public DateTime? PasswordUpdated { get; set; }
    }
}