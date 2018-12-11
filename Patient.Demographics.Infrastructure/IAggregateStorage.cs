using System;
using System.Threading.Tasks;
using Patient.Demographics.Domain;

namespace Patient.Demographics.Infrastructure
{
    public interface IAggregateStorage<T> where T : AggregateRoot
    {
        Task SaveAsync(T aggregate);
        Task<T> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}