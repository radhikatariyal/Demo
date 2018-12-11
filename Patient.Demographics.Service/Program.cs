using Patient.Demographics.Configuration.Extensions;
using Patient.Demographics.Service;
using Patient.Demographics.Service.WindsorInstallers;
using Castle.Windsor;

namespace Patient.Demographics.Service
{
    class Program
    {
        private static readonly IWindsorContainer WindsorContainer = new ServiceContainer(new [] { UseAssembly.This()});

        static void Main(string[] args)
        {
            IStartup startup = WindsorContainer.Resolve<IStartup>();
            startup.ConfigureService();
        }

    }
}
