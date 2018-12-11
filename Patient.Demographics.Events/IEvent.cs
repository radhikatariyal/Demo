using System;

namespace Patient.Demographics.Events
{
    public interface IEvent
    {
        Guid AggregateId { get; set;  }
    }
}
