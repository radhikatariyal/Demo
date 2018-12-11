using System.Collections.Generic;
using Patient.Demographics.Common;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Repository.Dtos.Users;
using Patient.Demographics.Data.Entities;
using System.Globalization;
using System.Linq;

namespace Patient.Demographics.Infrastructure.Mappers
{
    public static class DtoMapper
    {
        public static UserDto Map(UserEntity user)
        {
            return new UserDto
            {
                Id = user.Id,
               
            };
        }

      
    }
}