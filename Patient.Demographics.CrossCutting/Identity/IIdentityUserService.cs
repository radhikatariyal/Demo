using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Patient.Demographics.CrossCutting.Identity
{
    public interface IIdentityUserService
    {
        Task<IdentityServiceResult> ValidateUserAsync(string username, string email);

        Task<IdentityServiceResult> CreateUserAsync(Guid userId, string username, string email, string mobileNumber);

        Task<IdentityServiceResult> CreateUserAsync(Guid userId, string username, string email, string mobileNumber, string password);

        Task<IdentityResult> AddUserToRoleAsync(Guid userId, string roleName);

        Task<bool> IsUserInRoleAsync(Guid userId, string role);

        Task<string> GenerateNewUsername(string firstName, string lastName);

        Task<IdentityResult> ActivateUserAsync(Guid userId, string password, string securityQuestion, string securityAnswer, string email, string mobileNumber);

        Task<ClaimsIdentity> GenerateUserIdentityAsync(string username, string password);

        Task<bool> IsEmailInUseAsync(string email);

        Task<bool> IsUserLockedOut(Guid userId);

        Task<IdentityResult> UnlockUser(Guid userId);

        Task<AspNetUser> GetUserByUsername(string username);
     
        OAuthResult GenerateOAuthResult(ClaimsIdentity identity, bool isAdmin, string username, Guid userid, string url, string audienceId, string audienceSecret, bool isPasswordExpired);

        Task RecordLoginFailure(Guid userId, int maxAttempts);
        
        Task<bool> ValidateSecurityAnswer(Guid userId, string securityAnswer);
        Task<IdentityResult> ChangePasswordAsync(Guid userId, string newPassword);
     //   Task<IdentityResult> ResetPasswordAsync(Guid userId, string newPassword, string oldPassword);
      
        Task<bool> IsPasswordSameAsOldPassword(Guid userId, string newPassword);

        Task<IdentityResult> UpdateSecurityQuestionAsync(Guid userId, string securityQuestion, string securityAnswer);

        Task<IdentityResult> UpdateUserProfileAsync(Guid userId, string newEmail, string mobileNumber);

        Task<AspNetUser> FindByUserIdAsync(Guid userId);

        Task<bool> CheckUserPasswordAsync(string username, string password);
        Task<ClaimsIdentity> GenerateUserIdentityUsingUserIdAsync(AspNetUser user);

        Task<IdentityResult> ResetAccessAttemptsAsync(Guid userId);

        Task<Guid> GetRoleIdByName(string roleName);
    }
}