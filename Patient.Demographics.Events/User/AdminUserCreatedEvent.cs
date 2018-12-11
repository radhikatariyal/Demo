
namespace Patient.Demographics.Events.User
{
    public class AdminUserCreatedEvent : Event
    {
        public AdminUserCreatedEvent(string forename, string suname, System.DateTimeOffset dateofbirth, bool gender, string mobileNm, string homeNumber, string workNum)
        {
            FirstName = forename;
            LastName = suname;
            DateOfBirth =dateofbirth;
            Gender = gender;
            MobileNumber = mobileNm;
            WorkNumber = workNum;
            HomeNumber = homeNumber;
            
        }
        public System.DateTimeOffset DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}