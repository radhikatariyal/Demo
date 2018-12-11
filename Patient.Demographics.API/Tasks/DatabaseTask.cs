using Patient.Demographics.Common.Settings;
using Patient.Demographics.Configuration;
using Bootstrap.Extensions.StartupTasks;

namespace Patient.Demographics.API.Tasks
{
    public class DatabaseTask : IStartupTask
    {
        private readonly IDataSettings _devSettings;
        private readonly IDatabaseInitialise _databaseInitialise;

        public DatabaseTask(IDataSettings devSettings, IDatabaseInitialise databaseInitialise)
        {
            _devSettings = devSettings;
            _databaseInitialise = databaseInitialise;
        }

        public void Run()
        {
            if (_devSettings.AutoMigrateDatabase)
            {
                _databaseInitialise.Run();
            }
        }

        public void Reset()
        {
        }
    }
}