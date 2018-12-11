using System.Configuration;

namespace Patient.Demographics.Service
{
    public interface IServiceSettings
    {
        string UploadStatusEndpointUrl { get; }

        string ProgramCataloguePublishEndpointUrl { get; }

        string ServiceName { get; }

        string ServiceDisplayName { get; }

        string ApiUrl { get; }

    }

    public class ServiceSettings : IServiceSettings
    {
        public string UploadStatusEndpointUrl => "v1.0/uploadsnotification/status";
        public string ProgramCataloguePublishEndpointUrl => "v1.0/notification/programcataloguepublishstatus";
        public string ServiceName => ConfigurationManager.AppSettings["app:ServiceName"];
        public string ServiceDisplayName => ConfigurationManager.AppSettings["app:DisplayName"];
        public string ApiUrl => ConfigurationManager.AppSettings["api:Url"];
    }
}