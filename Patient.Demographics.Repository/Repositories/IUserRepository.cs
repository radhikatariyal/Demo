
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Repository.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Patient.Demographics.Repository.Repositories
{
    public interface IUserRepository
    {
       
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<List<UserDto>> GetUsers();
    }
}