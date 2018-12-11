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

        //public AccountsController()

        //{
           
        //}
        public AccountsController(
            ICommandExecutor commandExecutor,
            IUserRepository userRepository,
            IIdentityUserService identityUserService
           )

        {
            _commandExecutor = commandExecutor;
            _userRepository = userRepository;
            _identityUserService = identityUserService;
           
           
        }
        [HttpGet]
        [Route("getAllUsers", Name = "getAllUsers")]
        public async Task<HttpResponseMessage> GetAllUsers()
        {
            List<UserDto> UserDtoColl = new List<UserDto>();
            UserDtoColl = await _userRepository.GetUsers();
            return new HttpResponseMessage()
            {
                Content = new StringContent(
              XmlSerializeObject.SerializeObject(UserDtoColl),
              Encoding.UTF8,
              "text/html"
          )
            };
        }
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

        [Route("admins/{id:guid}", Name = "GetAdminById")]
        public async Task<HttpResponseMessage> GetAdmin(Guid id)
        {
            List<UserDto> UserDtoColl = new List<UserDto>();
            UserDto l  = await _userRepository.GetUserByIdAsync(id);
            return new HttpResponseMessage()
            {
                Content = new StringContent(
              XmlSerializeObject.SerializeObject(l),
              Encoding.UTF8,
              "text/html"
          )
            };
           // return Request.CreateResponse(HttpStatusCode.OK, XmlSerializeObject.SerializeObject(l), Configuration.Formatters.XmlFormatter);
            
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