using System.Configuration;
using Patient.Demographics.Common.Settings;

namespace Patient.Demographics.Common.Configuration
{
    public class AuthenticationServerConfigSettings : IAuthenticationServerSettings
    {
        public AuthenticationServerConfigSettings()
        {
            int maxFailedAttempts;
            int passwordExpiryDays; 

            if (int.TryParse(ConfigurationManager.AppSettings["as:MaxFailedAccessAttempts"], out maxFailedAttempts))
            {
                MaxFailedAttempts = maxFailedAttempts;
            }
            else
            {
                MaxFailedAttempts = 3;
            }

            if (int.TryParse(ConfigurationManager.AppSettings["as:PasswordExpiryDays"], out passwordExpiryDays))
            {
                PasswordExpiryDays = passwordExpiryDays;
            }
            else
            {
                PasswordExpiryDays = 90;
            }

            double authenticationExpirationMinutes;
            if (double.TryParse(ConfigurationManager.AppSettings["as:AuthenticationExpirationMinutes"], out authenticationExpirationMinutes))
            {
                AuthenticationExpirationMinutes = authenticationExpirationMinutes;
            }
            else
            {
                AuthenticationExpirationMinutes = 10;
            }

            double refreshTokenLifetimeMinutes;
            if (double.TryParse(ConfigurationManager.AppSettings["as:RefreshTokenLifetimeMinutes"], out refreshTokenLifetimeMinutes))
            {
                RefreshTokenLifetimeMinutes = passwordExpiryDays;
            }
            else
            {
                RefreshTokenLifetimeMinutes = 180;
            }
        }

        public string Issuer => ConfigurationManager.AppSettings["as:Issuer"];
        public string AudienceId => ConfigurationManager.AppSettings["as:AudienceId"];
        public string AudienceSecret => ConfigurationManager.AppSettings["as:AudienceSecret"];
        public int MaxFailedAttempts { get; }
        public int PasswordExpiryDays { get; }
        public double AuthenticationExpirationMinutes { get; }
        public double RefreshTokenLifetimeMinutes { get; }
    }
}