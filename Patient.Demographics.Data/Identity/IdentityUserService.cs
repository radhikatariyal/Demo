using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Patient.Demographics.Common;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Common.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Patient.Demographics.Common.Settings;

namespace Patient.Demographics.Data.Identity
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim> _identityDbContext;
        private readonly IApplicationUserManagerFactory _applicationUserManagerFactory;
        private readonly IAuthenticationServerSettings _authenticationServerSettings;

        public IdentityUserService(IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim> identityDbContext, IApplicationUserManagerFactory applicationUserManagerFactory, IAuthenticationServerSettings authenticationServerSettings)
        {
            _identityDbContext = identityDbContext;
            _applicationUserManagerFactory = applicationUserManagerFactory;
            _authenticationServerSettings = authenticationServerSettings;
        }

        public async Task<IdentityServiceResult> ValidateUserAsync(string username, string email)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var appUser = new AspNetUser() { UserName = username, Email = email };

            return new IdentityServiceResult
            {
                Result = await appUserManager.UserValidator.ValidateAsync(appUser),
                User = appUser
            };
        }

        public async Task<IdentityServiceResult> CreateUserAsync(Guid userId, string username, string email, string mobileNumber)
        {
            var appUser = new AspNetUser
            {
                Id = userId,
                UserName = username,
                Email = email,
             //   JoinDate = SystemDate.NowOffset().DateTime,
                LockoutEnabled = true,
                PhoneNumber = mobileNumber
            };

            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);

            return new IdentityServiceResult
            {
                Result = await appUserManager.CreateAsync(appUser),
                User = appUser
            };
        }

        public async Task<IdentityServiceResult> CreateUserAsync(Guid userId, string username, string email, string mobileNumber, string password)
        {
            var appUser = new AspNetUser
            {
                Id = userId,
                UserName = username,
                Email = email,
               // JoinDate = SystemDate.NowOffset().DateTime,
                LockoutEnabled = true,
                PhoneNumber = mobileNumber,
              //  PasswordUpdated = SystemDate.NowOffset().DateTime
            };

            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);

            var result = await appUserManager.CreateAsync(appUser);

            if (!result.Succeeded)
            {
                throw new ApplicationException("Error creating user.");
            }

            var addPasswordResult = await appUserManager.AddPasswordAsync(userId, password);
            if (!addPasswordResult.Succeeded)
            {
                throw new ApplicationException("Error adding password.");
            }

            return new IdentityServiceResult
            {
                Result = result,
                User = appUser
            };
        }

        public async Task<IdentityResult> AddUserToRoleAsync(Guid userId, string roleName)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            return await appUserManager.AddToRoleAsync(userId, roleName);
        }

        public async Task<bool> IsUserInRoleAsync(Guid userId, string role)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            return await appUserManager.IsInRoleAsync(userId, role);
        }

        public IdentityResult AddUserToRole(Guid userId, string roleName)
        {
            ArgumentValidator.EnsureIsNotNullOrWhitespace(roleName, nameof(roleName));
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            return appUserManager.AddToRole(userId, roleName);
        }

        public async Task<string> GenerateNewUsername(string firstName, string lastName)
        {
            ArgumentValidator.EnsureIsNotNullOrWhitespace(firstName, nameof(firstName));
            ArgumentValidator.EnsureIsNotNullOrWhitespace(lastName, nameof(lastName));

            var queryResult = _identityDbContext.Database
                .SqlQuery<string>("SELECT dbo.fn_GenerateNewUserName(@firstName, @lastName)",
                    new SqlParameter("@firstName", firstName),
                    new SqlParameter("@lastName", lastName));

            return await queryResult.FirstAsync();
        }

        public async Task<IdentityResult> ActivateUserAsync(Guid userId, string password, string securityQuestion, string securityAnswer, string email, string  mobileNumber)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);

            var addPasswordResult = await appUserManager.AddPasswordAsync(userId, password);
            if (!addPasswordResult.Succeeded)
            {
                throw new ApplicationException("Error adding password.");
            }

            var user = await appUserManager.FindByIdAsync(userId);
            user.SecurityQuestion = securityQuestion;
            user.SecurityAnswer = new PasswordHasher().HashPassword(securityAnswer);
            user.Email = email;
            user.PhoneNumber = mobileNumber;
           // user.PasswordUpdated = SystemDate.NowOffset().DateTime;

            return await appUserManager.UpdateAsync(user);
        }

        public async Task<bool> IsEmailInUseAsync(string email)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var result = await appUserManager.FindByEmailAsync(email);

            return result != null;
        }

        public async Task<AspNetUser> GetUserByUsername(string username)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var result = await appUserManager.FindByNameAsync(username);

            return result;
        }

        public OAuthResult GenerateOAuthResult(ClaimsIdentity identity, bool isAdmin, string username, Guid userid, string url, string audienceId, string audienceSecret, bool isPasswordExpired)
        {
            var configExpiry = _authenticationServerSettings.AuthenticationExpirationMinutes;

            var ticket = new AuthenticationTicket(identity, null);

            ticket.Properties.IssuedUtc = DateTimeOffset.UtcNow;
            ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(configExpiry);
            ticket.Properties.AllowRefresh = true;

            var formatter = new JwtFormatter(url, audienceId, audienceSecret);         
            var token = formatter.Protect(ticket);

            return new OAuthResult()
            {
                access_token = token,
                expires_in = TimeSpan.FromMinutes(configExpiry).TotalSeconds.ToString(CultureInfo.InvariantCulture),
                username = username,
                userid = userid,
                isAdmin = isAdmin,
                isPasswordExpired = isPasswordExpired
            };
        }

        public async Task RecordLoginFailure(Guid userId, int maxAttempts)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            appUserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(36500);
            appUserManager.MaxFailedAccessAttemptsBeforeLockout = maxAttempts;
            appUserManager.UserLockoutEnabledByDefault = true;

            await appUserManager.AccessFailedAsync(userId);
        }

        public async Task<bool> IsUserLockedOut(Guid userId)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            return await appUserManager.IsLockedOutAsync(userId);
        }

        public async Task<IdentityResult> UnlockUser(Guid userId)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            return await appUserManager.SetLockoutEndDateAsync(userId, DateTimeOffset.MinValue);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(string username, string password)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var user = await appUserManager.FindAsync(username, password);

            if (user == null)
            {
                return null;
            }

            return await appUserManager.CreateIdentityAsync(user, "JWT");
        }

        public async Task<bool> CheckUserPasswordAsync(string username, string password)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var user = await appUserManager.FindAsync(username, password);
            return user != null;
        }

        public async Task<AspNetUser> FindByUsernameAsync(string username)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            return await appUserManager.FindByNameAsync(username);
        }

        public async Task<bool> ValidateSecurityAnswer(string userName, string securityAnswer)
        {
            var passwordHash = new PasswordHasher();

            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            AspNetUser user = await appUserManager.FindByNameAsync(userName);

            var hashSecurityAnswer = passwordHash.VerifyHashedPassword(user.SecurityAnswer, securityAnswer.ToUpper());

            return hashSecurityAnswer.ToString().Equals("Success");
        }

        public async Task<bool> ValidateSecurityAnswer(Guid userId, string securityAnswer)
        {
            var passwordHash = new PasswordHasher();

            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            AspNetUser user = await appUserManager.FindByIdAsync(userId);

            var hashSecurityAnswer = passwordHash.VerifyHashedPassword(user.SecurityAnswer, securityAnswer.ToUpper());

            return hashSecurityAnswer.ToString().Equals("Success");
        }
        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string newPassword)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var user = await appUserManager.FindByIdAsync(userId);
            var passwordHash = new PasswordHasher();
            user.PasswordHash = passwordHash.HashPassword(newPassword);
           // user.PasswordUpdated = SystemDate.NowOffset().DateTime;
            return await appUserManager.UpdateAsync(user);
        }
      
        public async Task<bool> IsPasswordSameAsOldPassword(Guid userId, string newPassword)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var user = await appUserManager.FindByIdAsync(userId);
            var passwordHash = new PasswordHasher();
            var verificationResult = passwordHash.VerifyHashedPassword(user.PasswordHash, newPassword);

            return verificationResult.ToString().Equals("Success");
        }

        public async Task<IdentityResult> UpdateSecurityQuestionAsync(Guid userId, string securityQuestion, string securityAnswer)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var user = await appUserManager.FindByIdAsync(userId);
            user.SecurityQuestion = securityQuestion;
            user.SecurityAnswer = new PasswordHasher().HashPassword(securityAnswer.ToUpper());
            return await appUserManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(Guid userId, string newEmail, string mobileNumber)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            var user = await appUserManager.FindByIdAsync(userId);
            user.Email = newEmail;
            user.PhoneNumber = mobileNumber;
            return await appUserManager.UpdateAsync(user);
        }

        public async Task<AspNetUser> FindByUserIdAsync(Guid userId)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            return await appUserManager.FindByIdAsync(userId);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityUsingUserIdAsync(AspNetUser user)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);

            if (user == null)
            {
                return null;
            }

            return await appUserManager.CreateIdentityAsync(user, "JWT");
        }

        public async Task<IdentityResult> ResetAccessAttemptsAsync(Guid userId)
        {
            var appUserManager = _applicationUserManagerFactory.CreateApplicationUserManager(_identityDbContext);
            
            var user = await appUserManager.FindByIdAsync(userId);
            user.AccessFailedCount = 0;
            return await appUserManager.UpdateAsync(user);
        }

        public async Task<Guid> GetRoleIdByName(string roleName)
        {
            var roleManager = new RoleManager<AspNetRole, Guid>(new RoleStore<AspNetRole, Guid, AspNetUserRole>(_identityDbContext));
            var role = await roleManager.FindByNameAsync(roleName);
            return role.Id;
        }}
}