using Patient.Demographics.Common.Extensions;
using Patient.Demographics.Events;
using Patient.Demographics.Infrastructure;
using Castle.MicroKernel;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Demographics.Configuration
{
    public class WindsorEventHandlerFactory : IEventHandlerFactory
    {
        private readonly IKernel _kernel;
        private readonly IList<object> _eventHandlers;

        public WindsorEventHandlerFactory(IKernel kernel)
        {
            _kernel = kernel;
            _eventHandlers = new List<object>();
        }

        public IEnumerable<IEventHandler<T>> GetHandlers<T>(Event @event)
            where T : Event
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());

            var eventHandlers = _kernel.ResolveAll(handlerType)
                                       .Cast<IEventHandler<T>>()
                                       .ToList();

            _eventHandlers.AddRange(eventHandlers);

            return eventHandlers;
        }

        public void Dispose()
        {
            foreach (var eventHandler in _eventHandlers)
            {
                _kernel.ReleaseComponent(eventHandler);
            }
        }
    }
}