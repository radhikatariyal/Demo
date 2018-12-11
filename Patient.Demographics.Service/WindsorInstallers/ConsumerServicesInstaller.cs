using Patient.Demographics.Service.ConsumerServices;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Service.WindsorInstallers
{
    public class ConsumerServicesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterServiceWithLoggingInterceptor<IUploadStatusConsumerService, UploadStatusConsumerService>(container);
            RegisterServiceWithLoggingInterceptor<IErrorLogConsumerService, ErrorLogConsumerService>(container);
            //RegisterServiceWithLoggingInterceptor<IProgramCataloguePublishConsumerService, ProgramCataloguePublishConsumerService>(container);
        }

        private static void RegisterServiceWithLoggingInterceptor<TInterface, TClass>(IWindsorContainer container)
            where TInterface : class
            where TClass : TInterface
        {
            container.Register(Component.For<TInterface>()
                                        .ImplementedBy<TClass>()
                                        .LifestyleTransient()
                                        .Interceptors(InterceptorReference.ForKey("LoggingInterceptor")).First);
        }
    }
}