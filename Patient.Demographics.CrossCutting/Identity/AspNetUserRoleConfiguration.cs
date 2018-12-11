using System.Data.Entity.ModelConfiguration;

namespace Patient.Demographics.CrossCutting.Identity
{
    public class AspNetUserRoleConfiguration : EntityTypeConfiguration<AspNetUserRole>
    {
        public AspNetUserRoleConfiguration()
        {
            HasKey(e => new { e.RoleId, e.UserId });
        }
    }
}