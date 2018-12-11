using Patient.Demographics.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Patient.Demographics.Infrastructure
{
    public sealed class Repository<T> : IRepository<T> where T : AggregateRoot
    {
        private readonly IAggregateEventsHandler _aggregateEventsHandler;
        private readonly IAggregateStorageFactory _aggregateStorageFactory;

        public Repository(IAggregateEventsHandler aggregateEventsHandler, IAggregateStorageFactory aggregateStorageFactory)
        {
            _aggregateEventsHandler = aggregateEventsHandler;
            _aggregateStorageFactory = aggregateStorageFactory;
        }

        public async Task SaveAsync(T aggregate, Guid lastUpdatedByUserId)
        {
            if (aggregate.GetUnhandledEvents().Any())
            {
                IAggregateStorage<T> aggregateStorage = CreateAggregateStorage();

                try
                {
                    if (aggregate.State == AggregateState.Deleted)
                    {
                        await aggregateStorage.DeleteAsync(aggregate.Id);
                    }
                    else if (aggregate.State != AggregateState.NotModified)
                    {
                        await aggregateStorage.SaveAsync(aggregate);
                    }

                    await FireEvents(aggregate, lastUpdatedByUserId);

                    if (aggregate.State != AggregateState.Deleted)
                    {
                        aggregate.ClearUnhandledEvents();
                    }
                }
                catch (Exception ex)
                {
                    throw new RepositoryException("Error saving aggregate in Repository", ex);
                }
                finally
                {
                    _aggregateStorageFactory.Destroy(aggregateStorage);
                }
            }
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var aggregateStorage = CreateAggregateStorage();
            try
            {
                var item = await aggregateStorage.GetByIdAsync(id);

                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error getting aggregate from Repository", ex);
            }
            finally
            {
                _aggregateStorageFactory.Destroy(aggregateStorage);
            }
        }

        private IAggregateStorage<T> CreateAggregateStorage()
        {
            var aggregateStorage = _aggregateStorageFactory.Create<T>();
            if (aggregateStorage == null)
            {
                throw new AggregateException($"Aggregate Storage for type {typeof(T)} not found");
            }
            return aggregateStorage;
        }

        private async Task FireEvents(T aggregate, Guid lastUpdatedByUserId)
        {
            await _aggregateEventsHandler.HandleAll(aggregate, lastUpdatedByUserId);
        }
    }
}
