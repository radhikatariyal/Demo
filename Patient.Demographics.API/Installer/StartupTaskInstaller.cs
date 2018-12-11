
using Bootstrap.Extensions.StartupTasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Patient.Demographics.Configuration.Extensions;

namespace Patient.Demographics.API.Installers
{
    public class StartupTaskInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(UseAssembly.InApplicationDirectory("Patient.Demographics")
                .BasedOn<IStartupTask>()
                .LifestyleTransient().WithServiceFirstInterface());
        }
    }
}