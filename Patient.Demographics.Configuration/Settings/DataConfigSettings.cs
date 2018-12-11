using System.Configuration;
using Patient.Demographics.Common.Settings;

namespace Patient.Demographics.Configuration.Settings
{ 

    public class DataConfigSettings : IDataSettings
    {
        public DataConfigSettings()
        {
            bool autoMigrateDb;

            if(bool.TryParse(ConfigurationManager.AppSettings["dev:AutoMigrateDb"], out autoMigrateDb))
            {
                AutoMigrateDatabase = autoMigrateDb;
            }
            else
            {
                AutoMigrateDatabase = false;
            }
        }

        public bool AutoMigrateDatabase { get; }
    }
}