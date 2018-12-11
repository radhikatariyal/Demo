using System.Data.Entity.ModelConfiguration;
using Patient.Demographics.Data.Entities;

namespace Patient.Demographics.Data.EntityConfigurations
{
    public class UserEntityConfiguration : EntityTypeConfiguration<UserEntity>
    {
        public UserEntityConfiguration()
        {
            ToTable("UserAccounts").HasKey(t => t.Id);
            Property(t => t.Forename).IsRequired().HasMaxLength(50);
            Property(t => t.Surname).IsRequired().HasMaxLength(50);
            Property(t => t.MobileNumber);
            Property(t => t.WorkNumber);
            Property(t => t.HomeNumber);
            Property(t => t.Gender).IsRequired();
            Property(t => t.DateOfBirth);
            HasRequired(t => t.ApplicationUser).WithRequiredPrincipal();

        }
    }
}
