using System.Threading.Tasks;

namespace Patient.Demographics.Events
{
    public interface IEventHandler<in TEvent> where TEvent : Event
    {
        Task HandleAsync(TEvent @event);
    }
}