using Patient.Demographics.Common;
using Patient.Demographics.Domain;
using Patient.Demographics.Events.User;
using System;
using System.Collections.Generic;

namespace Patient.Demographics.Domain
{
    [Serializable]
    public sealed class User : AggregateRoot
    {
        public DateTimeOffset DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public static User CreateExisting(Guid id, string forename, string suname, string dateofbirth, bool gender, string mobileNm, string phoneNum, string workNum)
        {
            return new User(id, forename, suname, dateofbirth, gender, mobileNm, phoneNum, workNum);
        }


        public static User CreateNewUser(string forename, string suname, string dateofbirth, bool gender, string mobileNm, string homeNumber, string workNum)
        {
            var user = new User
            {
                Id = Identifier.NewId(),
                FirstName = forename,
                LastName = suname,
                DateOfBirth = Convert.ToDateTime(dateofbirth),
                Gender = gender,
                MobileNumber = mobileNm,
                WorkNumber = workNum,
                HomeNumber = homeNumber
            };

            user.LoadEvent(new AdminUserCreatedEvent(user.FirstName, user.LastName, user.DateOfBirth, user.Gender, user.MobileNumber, user.HomeNumber, user.WorkNumber));
            user.State = AggregateState.New;

            return user;
        }

        private User()
        {

        }
        private User(Guid id, string forename, string suname, string dateofbirth, bool gender, string mobileNm, string homeNumber, string workNum)
        {
            Id = id;
            FirstName = forename;
            LastName = suname;
            DateOfBirth = Convert.ToDateTime(dateofbirth);
            Gender = gender;
            MobileNumber = mobileNm;
            WorkNumber = workNum;
            HomeNumber = homeNumber;
            State = AggregateState.NotModified;

        }

    }
}