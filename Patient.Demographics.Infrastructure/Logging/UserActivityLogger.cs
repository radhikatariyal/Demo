using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.Events;
using Patient.Demographics.Data;
using Newtonsoft.Json;
using Patient.Demographics.Data.Entities;
//using Patient.Demographics.Data.Entities.Users;
using System.Web;

namespace Patient.Demographics.Infrastructure.Logging
{
    public class UserActivityLogger : IUserActivityLogger
    {
        private readonly ISqlDataContext _sqlDataContext;

        public UserActivityLogger(ISqlDataContext sqlDataContext)
        {
            _sqlDataContext = sqlDataContext;
        }

        public async Task InsertAuditLogAsync(Event activity)
        {
            await _sqlDataContext.SaveChangesAsync();
        }
    }
}

