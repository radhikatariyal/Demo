using Patient.Demographics.CommandHandlers.Users;
using Patient.Demographics.Commands;
using Patient.Demographics.Infrastructure;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Configuration
{
    public class CqrsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ICommandHandlerFactory>().AsFactory().LifestyleTransient());

            container.Register(Component.For<ICommandExecutor>().ImplementedBy<CommandExecutor>().LifestyleTransient());

            container.Register(Classes.FromAssemblyContaining<CreateAdminUserCommandHandler>()
                                      .BasedOn<ICommandHandler>()
                                      .WithServiceFirstInterface()
                                      .LifestyleTransient());
        }
    }
}