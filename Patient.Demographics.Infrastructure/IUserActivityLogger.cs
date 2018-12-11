using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patient.Demographics.Events;

namespace Patient.Demographics.Infrastructure
{
    public interface IUserActivityLogger
    {
        Task InsertAuditLogAsync(Event activity);
    }
}
