using System.Data.Entity.Migrations;
using Patient.Demographics.Common.Settings;
using Patient.Demographics.Data.Identity;

namespace Patient.Demographics.Configuration
{
    public interface IDatabaseInitialise
    {
        void Run();
    }

    public class DatabaseInitialise : IDatabaseInitialise
    {
        public void Run()
        {
            var sqlConfig = new Data.Migrations.Configuration();
            sqlConfig.AutomaticMigrationDataLossAllowed = true;
            var sqlDbMigrator = new DbMigrator(sqlConfig);
            sqlDbMigrator.Update();
        }
    }
}