//using System.Configuration;

//namespace Patient.Demographics.Service.Jobs.Emails
//{
//    public interface IEmailJobSettings
//    {
//        int RunAtHour { get; }
//        int RunAtMinute { get; }
//    }
//    public class EmailJobSettings : IEmailJobSettings
//    {
//        public EmailJobSettings()
//        {
//            var recalculateString = ConfigurationManager.AppSettings["job:ReminderEmailSchedule"];
//            if (!string.IsNullOrEmpty(recalculateString))
//            {
//                var timeSplit = recalculateString.Split(':');
//                int runAtHour;
//                int.TryParse(timeSplit[0], out runAtHour);
//                int runAtMinute;
//                int.TryParse(timeSplit[1], out runAtMinute);

//                RunAtHour = runAtHour;
//                RunAtMinute = runAtMinute;
//            }
//            else
//            {
//                RunAtHour = 12;
//                RunAtMinute = 0;
//            }
//        }

//        public int RunAtHour { get; }
//        public int RunAtMinute { get; }
//    }
//}