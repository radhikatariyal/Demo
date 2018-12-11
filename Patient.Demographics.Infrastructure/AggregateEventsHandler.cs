using System;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.Domain;

namespace Patient.Demographics.Infrastructure
{
    public interface IAggregateEventsHandler
    {
        Task HandleAll(AggregateRoot aggregate, Guid lastUpdatedByUserId);
    }

    public class AggregateEventsHandler : IAggregateEventsHandler
    {
        private readonly IEventBus _eventBus;
        private readonly IUserActivityLogger _userActivityLogger;

        public AggregateEventsHandler(IEventBus eventBus, IUserActivityLogger userActivityLogger)
        {
            _eventBus = eventBus;
            _userActivityLogger = userActivityLogger;
        }

        public async Task HandleAll(AggregateRoot aggregate, Guid lastUpdatedByUserId)
        {
            foreach (var eventObject in aggregate.GetUnhandledEvents().ToList())
            {
                eventObject.AggregateId = aggregate.Id;
                eventObject.UpdatedBy = lastUpdatedByUserId;
                await _eventBus.PublishAsync(ChangeTo(eventObject, eventObject.GetType()));
                await _userActivityLogger.InsertAuditLogAsync(eventObject);
            }

         
        }

        private static dynamic ChangeTo(dynamic source, Type dest)
        {
            return System.Convert.ChangeType(source, dest);
        }
    }
}