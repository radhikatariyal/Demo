using System;
using Patient.Demographics.Common;

namespace Patient.Demographics.Repository.Dtos
{
    [Serializable]
    public class UserAccountDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string ActivationCode { get; set; }
        public bool LockedOut { get; set; }
        public bool Deleted { get; set; }
        public bool PasswordExpired { get; set; }
        public int AccessFailedCount { get; set; }
        
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Culture { get; set; }

        public DateTime JoinDate { get; set; }

        public bool IsRegistrationRemainderEmailSent { get; set; }

        public bool IsAdmin { get; set; }

        public bool MustChangePassword { get; set; }

        public Guid? SupplierId { get; set; }
    }
}