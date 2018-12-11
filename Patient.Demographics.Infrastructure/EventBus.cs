
using Patient.Demographics.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Patient.Demographics.Infrastructure
{
    public sealed class EventBus : IEventBus
    {
        private readonly IEventHandlerFactory _eventHandlerFactory;
     

        public EventBus(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
          
        }

        public async Task PublishAsync<T>(T @event) where T : Event
        {
            var eventHandlers = _eventHandlerFactory.GetHandlers<T>(@event).ToList();

            try
            {
                foreach (var eventHandler in eventHandlers)
                {
                    try
                    {
                        await eventHandler.HandleAsync(@event);
                    }
                    catch (Exception ex)
                    {
                        throw new EventException($"Error in EventHandler {eventHandler.GetType().FullName}", ex);
                    }
                }

               // await _messageQueue.PublishAfterTransactionCompletedAsync(@event);
            }
            catch (Exception ex)
            {
                throw new EventException("Error in Event Bus", ex);
            }
        }
    }
}