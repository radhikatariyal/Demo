namespace Patient.Demographics.Events
{
    public interface IEventStore
    {
        void Add<T, Handler>()
            where T : Event
            where Handler : IEventHandler<T>;
    }
}