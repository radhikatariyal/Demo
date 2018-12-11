using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class AspNetUserRole : IdentityUserRole<Guid>
    {
        [Key]
        [Column(Order = 1)]
        public override Guid UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public override Guid RoleId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AspNetUser User { get; set; }


        [ForeignKey(nameof(RoleId))]
        public virtual AspNetRole Role { get; set; }
    }
}