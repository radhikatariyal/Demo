using System.Configuration;
using Patient.Demographics.Common.Settings;

namespace Patient.Demographics.Configuration.Settings
{
    public class MessageQueueConfigSettings : IMessageQueueSettings
    {
        public string MessageQueueVHost => ConfigurationManager.AppSettings["rmq:VHost"];
        public string MessageQueueUsername => ConfigurationManager.AppSettings["rmq:Username"];
        public string MessageQueuePassword => ConfigurationManager.AppSettings["rmq:Password"];
    }
}