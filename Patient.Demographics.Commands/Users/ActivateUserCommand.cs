using System;
using System.ComponentModel.DataAnnotations;
using Patient.Demographics.Commands.Attributes;
using Newtonsoft.Json;

namespace Patient.Demographics.Commands.Users
{
    public class ActivateUserCommand : Command
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string ActivationCode { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string MobileNumber { get; set; }

        [Required]
        public string SecurityQuestion { get; set; }

        [Required]
        public string SecurityAnswer { get; set; }

        [Required]
        [DataType(DataType.Html)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Html)]
        public string ConfirmPassword { get; set; }

        [BoolFixedValue(true, nameof(TermsAccepted) + " must be true")]
        public bool TermsAccepted { get; set; }
    }
}