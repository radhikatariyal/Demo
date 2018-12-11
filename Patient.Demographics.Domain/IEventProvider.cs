using System.Collections.Generic;
using Patient.Demographics.Events;

namespace Patient.Demographics.Domain
{
    public interface IEventProvider
    {
        IEnumerable<Event> GetUnhandledEvents();
    }
}
