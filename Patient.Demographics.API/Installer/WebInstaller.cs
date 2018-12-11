using System.Reflection;
using Patient.Demographics.Configuration.Extensions;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.API.Installers
{
    public class WebInstaller : IWindsorInstaller
    {
        private Assembly[] _assemblies;

        public void SetAssembly(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
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