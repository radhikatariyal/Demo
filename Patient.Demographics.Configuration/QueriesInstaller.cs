using Patient.Demographics.Repository.Repositories;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Patient.Demographics.Queries.Users;

namespace Patient.Demographics.Configuration
{
    public class QueriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterQueryComponents(container);
        }

        private static void RegisterQueryComponents(IWindsorContainer container)
        {
              container.Register(Component.For<IUserRepository>().ImplementedBy<UserRepository>().LifestyleTransient());
            //container.Register(Component.For<IUserAccountRepository>().ImplementedBy<UserAccountRepository>().LifestyleTransient());
                 }

        //private static void RegisterRepositoryWithCachingInterceptor<TInterface, TClass>(IWindsorContainer container) where TInterface : class where TClass : TInterface
        //{
        //    container.Register(Component.For<TInterface>().ImplementedBy<TClass>().LifestyleTransient().Interceptors(InterceptorReference.ForKey("CachingInterceptor")).First);
        //}
    }
}