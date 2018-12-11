using BIWorldwide.GPSM.Common.Settings;
using BIWorldwide.GPSM.Repository.Repositories;
using Bootstrap.Extensions.StartupTasks;

namespace BIWorldwide.GPSM.API.Tasks
{
    public class CacheWarmupTask : IStartupTask
    {
        private readonly IDataSettings _dataSettings;

        public CacheWarmupTask(IDataSettings dataSettings)
        {
            _dataSettings = dataSettings;
        }

        public void Run()
        {
            if (_dataSettings.CacheQueries)
            {
            }
        }

        public void Reset()
        {
        }
    }
}