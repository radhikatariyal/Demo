using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Data.Entities;
using System.Linq;

namespace Patient.Demographics.Data
{
    public class QueryModelData : IQueryModelData
    {
        public QueryModelData(ISqlDataContext dbContext)
        {
            UsersQuery = dbContext.UsersEntities.AsNoTracking();

        }
        public IQueryable<UserEntity> UsersQuery { get; }
    }
}
