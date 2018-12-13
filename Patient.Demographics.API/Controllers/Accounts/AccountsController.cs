using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Patient.Demographics.Commands;
using Patient.Demographics.Commands.Users;
using Patient.Demographics.CrossCutting.Identity;
using Patient.Demographics.Repository;
using Patient.Demographics.Repository.Repositories;
using Patient.Demographics.API.Attributes;
using Patient.Demographics.API.Controllers;
using Patient.Demographics.API.Configuration;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;
using Patient.Demographics.CrossCutting.Security;
using System.Web;
using Patient.Demographics.Common;
using System.Globalization;
using System.Collections.ObjectModel;
using Patient.Demographics.CommandHandlers.Users;
using System.Net.Http;
using Patient.Demographics.Repository.Dtos.Users;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Patient.Demographics.Common.Serialization;
using System.Text;

namespace Patient.Demographics.Web.API.Controllers.Accounts
{
    [AllowAnonymous]
    [RoutePrefix("v1.0/accounts")]
    public class AccountsController : BaseApiController
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUserService _identityUserService;

        public AccountsController(
            ICommandExecutor commandExecutor,//injectinng commandExecutor to execute command
            IUserRepository userRepository//injectinng userrepo

           )

        {
            _commandExecutor = commandExecutor;
            _userRepository = userRepository;


        }

        //Fetch all users records 
        [HttpGet]
        [Route("getAllUsers", Name = "getAllUsers")]
        public async Task<HttpResponseMessage> GetAllUsers()
        {
            List<UserDto> UserDtoColl = new List<UserDto>();
            UserDtoColl = await _userRepository.GetUsers();
            if (UserDtoColl == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(
              XmlSerializeObject.SerializeObject(UserDtoColl),
              Encoding.UTF8,
              "text/html"
          )
            };
        }

        //Create new User
        [HttpPost]
        public async Task<IHttpActionResult> CreateUser(CreateAdminUserCommand command)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _commandExecutor.ExecuteAsync(command);

            return Ok();
        }

        //get user by Id
        [Route("admins/{id:guid}", Name = "GetAdminById")]
        public async Task<HttpResponseMessage> GetAdmin(Guid id)
        {
            List<UserDto> UserDtoColl = new List<UserDto>();
            UserDto user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(
              XmlSerializeObject.SerializeObject(user),
              Encoding.UTF8,
              "text/html"
          )
            };

        }

        public string CreateXML(Object YourClassObject)
        {
            XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
                                                      // Initializes a new instance of the XmlDocument class.          
            XmlSerializer xmlSerializer = new XmlSerializer(YourClassObject.GetType());
            // Creates a stream whose backing store is memory. 
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, YourClassObject);
                xmlStream.Position = 0;
                //Loads the XML document from the specified string.
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }
    }
}