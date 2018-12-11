
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Patient.Demographics.Repository.Dtos.Users
{

  
    public class UserDto
    {
       
        public Guid Id { get; set; }

        
        public DateTimeOffset DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Username { get; set; }
      
        public string FirstName { get; set; }
     
        public string LastName { get; set; }
      
    }
}