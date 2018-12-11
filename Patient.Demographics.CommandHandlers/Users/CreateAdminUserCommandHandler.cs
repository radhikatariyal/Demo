using Patient.Demographics.Common;
using Patient.Demographics.Common.Extensions;
using Patient.Demographics.Common.Validation;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Domain;
using Patient.Demographics.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.Commands.Users;
using Patient.Demographics.Repository.Repositories;
using System.Collections.Generic;

namespace Patient.Demographics.CommandHandlers.Users
{
    public class CreateAdminUserCommandHandler : CreateUserCommandHandlerBase, ICommandHandler<CreateAdminUserCommand>
    {
        private readonly IRepository<User> _repository;
        private readonly IUserRepository _userRepository;
        public CreateAdminUserCommandHandler(IRepository<User> repository, IIdentityUserService identityUserService, IUserRepository userSetRepository) : base(identityUserService)
        {
            _repository = repository;
            _userRepository = userSetRepository;
        }

        public async Task HandleAsync(CreateAdminUserCommand command)
        {
              ArgumentValidator.EnsureIsNotNull(command, nameof(command));

            var role = User.CreateNewUser(command.ForeName,command.SurName,command.DateOfBirth.ToString(),command.Gender,command.MobileNumber,command.HomeNumber,command.WorkNumber);
            await _repository.SaveAsync(role, command.CommandIssuedByUserId);
        }

    }
}


