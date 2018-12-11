using Patient.Demographics.Configuration.Extensions;
using Castle.Facilities.Startable;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Configuration
{
    public class ApplicationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.AddFacility<TypedFactoryFacility>();
            container.AddFacility<StartableFacility>(f => f.DeferredStart());
            container.Kernel.Resolver
                 .AddSubResolver(new CollectionResolver(container.Kernel, true));
            container.Install
          (
              UseAssembly.This()
                  .FindInstallers()
                  .Exclude<ApplicationInstaller>()
          );
        }
    }
}