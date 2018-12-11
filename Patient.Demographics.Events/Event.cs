using System;

namespace Patient.Demographics.Events
{
    [Serializable]
    public abstract class Event : IEvent
    {
        public Guid AggregateId { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
