using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Patient.Demographics.Commands.Users
{

    public class CreateAdminUserCommand : Command
    {
        public Guid Id { get; set; }    
       
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string ForeName { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string SurName { get; set; }
        
        public DateTimeOffset DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string MobileNumber { get; set; }

    }
}