using Topshelf;

namespace Patient.Demographics.Service
{
    public class Startup : IStartup
    {
        private readonly ISchedulingService _schedulingService;
        private readonly IServiceSettings _configAppSettings;

        public Startup(ISchedulingService schedulingService, IServiceSettings configAppSettings)
        {
            _schedulingService = schedulingService;
            _configAppSettings = configAppSettings;
        }

        public void ConfigureService()
        {
            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<ISchedulingService>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(hostSettings => _schedulingService);
                    serviceConfigurator.WhenStarted(service => service.Start());
                    serviceConfigurator.WhenStopped(service => service.Stop());
                });
                hostConfigurator.SetDescription("Service for handling the \"GPSM\" schedulers");
                hostConfigurator.SetDisplayName(_configAppSettings.ServiceDisplayName);
                hostConfigurator.SetServiceName(_configAppSettings.ServiceName);
                hostConfigurator.StartAutomatically();
            });
        }
    }
}
