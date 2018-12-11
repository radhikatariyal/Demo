using System;
using System.Collections.Generic;
using BIWorldwide.GPSM.Common;

namespace BIWorldwide.GPSM.Data
{
    public static class SampleUsers
    {
        public static List<SampleUser> Users { get; set; }

        static SampleUsers()
        {
            Users = new List<SampleUser>
            {
                new SampleUser
                {
                    Id = Guid.Parse("38bb71fe-ad87-499e-873a-a67800ac2a28"),
                    Username = "tariyal",
                    FirstName = "GPSM",
                    LastName = "Admin",
                    Email = "gpsm@biworldwide.com",
                    IsAdmin = true
                }
            };
        }

        public class SampleUser
        {
            public SampleUser()
            {
                TermsAccepted = true;
            }
            public Guid Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ActivationCode { get; set; }
            public bool TermsAccepted { get; set; }
            public bool IsAdmin { get; set; }
        }
    }
}
