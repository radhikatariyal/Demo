
using Patient.Demographics.Events;
using Patient.Demographics.Infrastructure;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Patient.Demographics.Configuration
{
    public class EventsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IEventHandlerFactory>()
                                        .ImplementedBy<WindsorEventHandlerFactory>()
                                        .LifestyleTransient());

            container.Register(Component.For<IAggregateEventsHandler>()
                                .ImplementedBy<AggregateEventsHandler>()
                                .LifestyleTransient());

            container.Register(Component.For<IEventBus>()
                                        .ImplementedBy<EventBus>()
                                        .LifestyleTransient());

            container.Register(Component.For<IEventStore>()
                                        .ImplementedBy<EventStore>()
                                        .LifestyleTransient());

                 }
    }
}