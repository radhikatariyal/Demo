using System.Reflection;
using Patient.Demographics.Configuration.Extensions;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Service.WindsorInstallers
{
    public class ServiceInstaller : IWindsorInstaller
    {
        private Assembly[] _assemblies;

        public void SetAssembly(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Install Web Application Installers
            container.Install
            (
                UseAssembly
                    .This()
                    .FindInstallers()
                    .Exclude<ServiceInstaller>()
            );

            if (_assemblies != null)
            {
                foreach (var assembly in _assemblies)
                {
                    container.Install
                        (
                            assembly.FindInstallers()
                        );
                }
            }
        }
    }
}