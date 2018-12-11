using Patient.Demographics.Events;
using System;
using System.Collections.Generic;

namespace Patient.Demographics.Domain
{
    [Serializable]
    public abstract class AggregateRoot : IEventProvider
    {
        private readonly List<Event> _events;
        
        protected AggregateRoot()
        {
            _events = new List<Event>();
        }

        public Guid Id { get; protected set; }

        public AggregateState State { get; protected set; }

        protected internal void LoadEvent(Event e)
        {
            _events.Add(e);
        }

        public IEnumerable<Event> GetUnhandledEvents()
        {
            return _events;
        }

        public virtual void Delete()
        {
            State = AggregateState.Deleted;
        }

        public void ClearUnhandledEvents()
        {
            State = AggregateState.NotModified;
            _events.Clear();
        }
    }
}
