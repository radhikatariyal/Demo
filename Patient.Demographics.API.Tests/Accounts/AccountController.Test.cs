using Patient.Demographics.Commands;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.Web.API.Controllers.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using Xunit;
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
            _identityUserService = Substitute.For<IIdentityUserService>();
            _userAccountRepository = Substitute.For<IUserAccountRepository>();
            _communicationRepository = Substitute.For<ICommunicationRepository>();

            _sut = new AccountsController(_commandExecutor, _userRepository, _identityUserService, _userAccountRepository, _communicationRepository)
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
            var result = await _sut.GetAdmin(Guid.NewGuid());
            var response = await result.ExecuteAsync(new CancellationToken());

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

    }
}
