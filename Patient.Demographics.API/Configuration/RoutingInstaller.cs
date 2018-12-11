using Patient.Demographics.API.Controllers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Patient.Demographics.Common.Interfaces;

namespace Patient.Demographics.API.Configuration
{
    public class RoutingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Register(Component.For<ISignalRProxy>().ImplementedBy<SignalRProxy>().LifestyleTransient());
        }
    }
}