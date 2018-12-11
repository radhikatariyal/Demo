using System;
using System.Threading.Tasks;

namespace Patient.Demographics.Domain
{
    public interface IRepository<T> where T : AggregateRoot
    {
        Task SaveAsync(T aggregate, Guid lastUpdatedByUserId);
        Task<T> GetByIdAsync(Guid id);
        //Task InvalidateObject(Guid id);
    }
}
 