using System.Reflection;
using Patient.Demographics.Configuration;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

namespace Patient.Demographics.Service.WindsorInstallers
{
    public class ServiceContainer : WindsorContainer
    {
        public ServiceContainer(Assembly[] assemblies = null)
        {
            AddFacility<TypedFactoryFacility>();
            Kernel.Resolver
              .AddSubResolver(new CollectionResolver(Kernel, true));
            Install(
                new ServiceInstaller(),
                new EventsInstaller(),
                new CrossCuttingInstaller(),
                new QueriesInstaller(),
                new CqrsInstaller(),
                new SettingsInstaller(),
                new InterceptorsInstaller(),
                new InfrastructureInstaller());
        }
    }
}