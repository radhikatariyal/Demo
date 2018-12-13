
using Patient.Demographics.Web.API.Controllers.Accounts;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Commands;
using BIWorldwide.GPSM.Tests.Common;
using Patient.Demographics.Repository.Dtos.Users;
using System.Net;

namespace Patient.Demographics.API.Tests.Accounts
{
    class AccountsControllerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IIdentityUserService _identityUserService;
        private readonly IUserAccountRepository _userAccountRepository;
       // private readonly ICommunicationRepository _communicationRepository;
        private readonly AccountsController _sut;


        public AccountsControllerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _commandExecutor = Substitute.For<ICommandExecutor>();   
           
            _sut = new AccountsController(_commandExecutor, _userRepository)
            {
                ControllerContext = ControllersHelper.CreateControllerContext()
            };

            _sut.AddBaseUrlLink();
            _sut.SetUserId(Guid.NewGuid());
        }

        [Fact]
        public async void GetAdmin_ReturnsNotFoundForNotAvailable()
        {
            //Arrange
            _userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(Task.Run<UserDto>(() => (UserDto)null));

            //Act
            var result = await _sut.GetAllUsers();
            

            //Assert
            Assert.Equal(HttpStatusCode.Found, result.StatusCode);
        }

    }
}
