using System.Configuration;
using Patient.Demographics.Common.Settings;

namespace Patient.Demographics.Configuration.Settings
{
    public class WebsiteConfigSettings : IWebsiteSettings
    {
        public WebsiteConfigSettings()
        {
            bool apiWarmup;

            if (bool.TryParse(ConfigurationManager.AppSettings["dev:AutomaticWarmup"], out apiWarmup))
            {
                ApiAutoWarmup = apiWarmup;
            }
            else
            {
                ApiAutoWarmup = true;
            }
        }

        public string ApiUrl => ConfigurationManager.AppSettings["api:Url"];
        public string AuthenticationServer => ConfigurationManager.AppSettings["auth:Server"];
        public virtual string SiteRoot { get; }
        public bool ApiAutoWarmup { get; }
    }
}