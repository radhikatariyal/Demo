using Patient.Demographics.Service.FileUploads;
//using Patient.Demographics.Service.Payments;
using Patient.Demographics.Service.Notifiers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MassTransit;

namespace Patient.Demographics.Service.WindsorInstallers
{
    public class ServiceWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IStartup>().ImplementedBy<Startup>().LifestyleTransient());
            container.Register(Classes.FromThisAssembly().BasedOn<IConsumer>().LifestyleTransient());
            container.Register(Component.For<IServiceSettings>().ImplementedBy<ServiceSettings>().LifestyleTransient());
            container.Register(Component.For<ISchedulingService>().ImplementedBy<SchedulingService>().LifestyleTransient());
            container.Register(Component.For<IUploadsNotifier>().ImplementedBy<RestUploadNotifier>().LifestyleTransient());
        }
    }
}