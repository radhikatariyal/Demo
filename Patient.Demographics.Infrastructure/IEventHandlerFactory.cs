using System;
using Patient.Demographics.Events;
using System.Collections.Generic;

namespace Patient.Demographics.Infrastructure
{
    public interface IEventHandlerFactory : IDisposable
    {
        IEnumerable<IEventHandler<T>> GetHandlers<T>(Event @event) where T : Event;
    }
}