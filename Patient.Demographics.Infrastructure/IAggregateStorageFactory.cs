using Patient.Demographics.Domain;

namespace Patient.Demographics.Infrastructure
{
    public interface IAggregateStorageFactory
    {
        /// <summary>
        /// Creates the correct aggregate storage class
        /// </summary>
        /// <typeparam name="T">The aggregate storage class to be returned</typeparam>
        /// <returns>A aggregate storage handler</returns>
        IAggregateStorage<T> Create<T>() where T : AggregateRoot;

        /// <summary>
        /// Destroys the aggregate storage class
        /// </summary>
        /// <typeparam name="T">The aggregate storage to be destroyed</typeparam>
        /// <param name="command">The instance of the aggregate storage to destroy</param>
        void Destroy<T>(T command);
    }
}