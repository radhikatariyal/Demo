using Patient.Demographics.Domain;
using Patient.Demographics.Infrastructure.Mappers;
using Patient.Demographics.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Patient.Demographics.Infrastructure.Storage.Aggregates
{
    public class UserStorage : IAggregateStorage<User>
    {
        private readonly ISqlDataContext _dbContext;

        public UserStorage(ISqlDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveAsync(User aggregate)
        {
            var userEntity = AggregateMapper.Map(aggregate);

            if (aggregate.State == AggregateState.New)
            {
                _dbContext.UsersEntities.Add(userEntity);
            }
            else
            {
                _dbContext.SetModified(userEntity);
            }

            await _dbContext.SaveChangesAsync();
        }

    

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}