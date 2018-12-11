using System.Threading.Tasks;
using Patient.Demographics.Events;

namespace Patient.Demographics.Infrastructure
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : Event;
    }
}