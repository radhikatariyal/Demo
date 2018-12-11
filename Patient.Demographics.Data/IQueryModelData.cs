using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Data.Entities;
using System.Linq;

namespace Patient.Demographics.Data
{
    public interface IQueryModelData
    {
        IQueryable<UserEntity> UsersQuery { get; }
    }
}