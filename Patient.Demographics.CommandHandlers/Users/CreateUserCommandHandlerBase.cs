using System;
using System.Threading.Tasks;
using Patient.Demographics.CrossCutting.Identity;

namespace Patient.Demographics.CommandHandlers.Users
{
    public abstract class CreateUserCommandHandlerBase
    {
        protected readonly IIdentityUserService IdentityUserService;

        protected CreateUserCommandHandlerBase( IIdentityUserService identityUserService)
        {
            
            IdentityUserService = identityUserService;
        }

        protected async Task CreateAspNetIdentityUserAsync(string username, string email, Guid userId, string role, string mobileNumber)
        {
            var userResult = await IdentityUserService.CreateUserAsync(userId, username, email, mobileNumber);
            if (!userResult.Result.Succeeded)
            {
                throw new IdentityUserResultException(userResult.Result, "Error Creating ASPNET Identity User");
            }

            var addToRoleResult = await IdentityUserService.AddUserToRoleAsync(userResult.User.Id, role);
            if (!addToRoleResult.Succeeded)
            {
                throw new IdentityUserResultException(addToRoleResult, "Error occurred while adding user to role.");
            }
        }
    }
}