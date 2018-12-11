using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.CrossCutting.Logger;
using Patient.Demographics.Infrastructure.Logging;
using Patient.Demographics.Data;
using Patient.Demographics.Data.Identity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using Patient.Demographics.Common.Settings;

namespace Patient.Demographics.Configuration
{
    public class CrossCuttingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //RegisterCachingComponents(container);
            RegisterLoggingComponents(container);
            RegisterSecurityComponents(container);
        }
        
        private static void RegisterLoggingComponents(IWindsorContainer container)
        {
            container.Register(Component.For<ILogger>()
                                        .ImplementedBy<SqlLogger>()
                                        .LifestyleTransient());
        }

        private static void RegisterSecurityComponents(IWindsorContainer container)
        {
            container.Register(Component.For<IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim>>()
                                        .ImplementedBy<SqlDataContext>()
                                        .LifestyleTransient()
                                        .Named("IdentityContext"));

            container.Register(Component.For<IIdentityUserService>()
                                        .ImplementedBy<IdentityUserService>()
                                        .LifestyleTransient());

            container.Register(Component.For<IApplicationUserManagerFactory>()
                                        .ImplementedBy<AspNetUserManagerFactory>()
                                        .LifestyleTransient());

           

            container.Register(Component.For<IRefreshtokenDataProtector>()
                                        .ImplementedBy<MachineKeyDataProtector>()
                                        .LifestyleTransient());

            container.Register(Component.For<ISecureDataFormat<AuthenticationTicket>>()
                                        .ImplementedBy<CustomTicketDataFormat>()
                                        .LifestyleTransient());
        }
    }
}