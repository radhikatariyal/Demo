using System;
using System.Collections.Generic;
using Patient.Demographics.Common;

namespace Patient.Demographics.Data
{
    public static class TestUsers
    {
        public static List<TestUser> Users { get; set; }

        static TestUsers()
        {
            Users = new List<TestUser>
            {
               new TestUser
                {
                    Id = Guid.Parse("F3642E5E-7411-444F-B23B-0D5212B0A6D2"),
                    Username = "gpsmadmin",
                 ForeName = "testsupplier",
                    SurName = "test",
                    DateOfBirth=SystemDate.NowOffset(),
                   Gender=true,
                   HomeNumber="011-273737373"
                },
                new TestUser
                {
                    Id = Guid.Parse("618E6FAB-3542-4360-9CA9-02485B652947"),
                    Username = "tariyal",
                ForeName = "testsupplier",
                    SurName = "test",
                    DateOfBirth=SystemDate.NowOffset(),
                   Gender=true,
                   HomeNumber="011-273737373"
                },

                new TestUser
                {
                    Id = Guid.Parse("417302B0-33A2-4E37-B022-6A301B2F31CE"),
                    Username = "kunhiram",
                    ForeName = "testsupplier",
                    SurName = "test",
                    DateOfBirth=SystemDate.NowOffset(),
                   Gender=true,
                   HomeNumber="011-273737373"
                },
                new TestUser
                {
                    Id = Guid.Parse("D94FFD8C-D59C-47FB-BB98-703435B304A4"),
                   ForeName = "testsupplier",
                    SurName = "test",
                    DateOfBirth=SystemDate.NowOffset(),
                   Gender=true,
                   HomeNumber="011-273737373"
                },
                 new TestUser
                {
                    Id = Guid.Parse("B94FFD8C-D59C-27FB-BB98-703435B304A4"),
                    Username = "testsupplier",
                    ForeName = "testsupplier",
                    SurName = "test",
                    DateOfBirth=SystemDate.NowOffset(),
                   Gender=true,
                   HomeNumber="011-273737373"
                                      
                },
                new TestUser
                {
                    Id = Guid.Parse("B78FFD8C-D59C-27FB-BB98-703435B304A4"),
                    Username = "supplier",
                  ForeName = "testsupplier",
                    SurName = "test",
                    DateOfBirth=SystemDate.NowOffset(),
                   Gender=true,
                   HomeNumber="011-273737373"
                },
                new TestUser
                {
                    Id = Guid.Parse("B90FFD8C-D59C-27FB-BB98-703435B304A4"),
                    ForeName = "testsupplier",
                    SurName = "test",
                    DateOfBirth=SystemDate.NowOffset(),
                   Gender=true,
                   HomeNumber="011-273737373"
                }
            };
        }

        public class TestUser
        {
            public TestUser()
            {
               // TermsAccepted = true;
            }
            public Guid Id { get; set; }
            public DateTimeOffset DateOfBirth { get; set; }
            public bool Gender { get; set; }
            public string HomeNumber { get; set; }
            public string WorkNumber { get; set; }
            public string MobileNumber { get; set; }
            public string Username { get; set; }

            public string ForeName { get; set; }

            public string SurName { get; set; }
        }
    }
}
