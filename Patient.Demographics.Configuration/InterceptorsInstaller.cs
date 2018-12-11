//using Patient.Demographics.CrossCutting.Caching;
using Patient.Demographics.CrossCutting.Logger;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Configuration
{
    public class InterceptorsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Register(Component.For<IInterceptor>()
            //                            .ImplementedBy<CachingInterceptor>()
            //                            .Named("CachingInterceptor")
            //                            .LifestyleTransient());

            container.Register(Component.For<IInterceptor>()
                                        .ImplementedBy<LoggingInterceptor>()
                                        .Named("LoggingInterceptor")
                                        .LifestyleTransient());
        }
    }
}