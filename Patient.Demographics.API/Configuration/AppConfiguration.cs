using System.Configuration;

namespace Patient.Demographics.API.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
            AcceptableDomains = ConfigurationManager.AppSettings["acceptableDomains"];
            ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string autoMigrateDbString = ConfigurationManager.AppSettings["dev:AutoMigrateDb"];
            bool autoMigrateDb;
            if (bool.TryParse(autoMigrateDbString, out autoMigrateDb))
            {
                AutoMigrateDatabase = autoMigrateDb;
            }
        }

        public string AcceptableDomains { get; }
        public bool AutoMigrateDatabase { get; }
        public string ConnectionString { get; }
    }
}