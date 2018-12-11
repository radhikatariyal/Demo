using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Patient.Demographics.Repository;
using Patient.Demographics.Repository.Dtos;
using Patient.Demographics.Repository.Dtos.Users;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Data.Entities;
using Patient.Demographics.Data;

namespace Patient.Demographics.Queries.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IQueryModelData _queryModelData;

        public UserRepository(IQueryModelData queryModelData)
        {
            _queryModelData = queryModelData;
        }

        public async Task<List<UserDto>> GetUsers()
        {           
            return await _queryModelData.UsersQuery.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.Forename,
                LastName = u.Surname,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                WorkNumber = u.WorkNumber,
                HomeNumber = u.HomeNumber,
                MobileNumber = u.MobileNumber
            }).ToListAsync();


        }
        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var result= await _queryModelData.UsersQuery.Where(u => u.Id == id).Select(u=>u).FirstOrDefaultAsync();
            
            var userDto = MapUser(result);
            return userDto;
        }


        private UserDto MapUser(UserEntity userEntity)
        {
            var userDto = new UserDto
            {
                Id = userEntity.Id,                
                FirstName = userEntity.Forename,
                LastName = userEntity.Surname,
                DateOfBirth= userEntity.DateOfBirth,
                Gender= userEntity.Gender,
                WorkNumber= userEntity.WorkNumber,
                HomeNumber= userEntity.HomeNumber,
                MobileNumber = userEntity.MobileNumber,
               
            };

            return userDto;
        }



    }
}