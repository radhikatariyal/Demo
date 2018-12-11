using Patient.Demographics.Domain;
using Patient.Demographics.Data.Entities;

using System.Collections.Generic;

namespace Patient.Demographics.Infrastructure.Mappers
{
    public static class AggregateMapper
    {
        public static UserEntity Map(User user)
        {
            var entity = new UserEntity
            {
                Id = user.Id,

            };

            return entity;
        }

        

    }
}