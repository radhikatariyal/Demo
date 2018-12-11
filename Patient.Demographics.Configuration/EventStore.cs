using Patient.Demographics.Events;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace Patient.Demographics.Configuration
{
    public class EventStore : IEventStore
    {
        private readonly IKernel _kernel;

        public EventStore(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Add<T, THandler>()
            where T : Event
            where THandler : IEventHandler<T>
        {
            _kernel.Register(Component.For<IEventHandler<T>>().ImplementedBy<THandler>().LifestyleTransient());
        }
    }
}