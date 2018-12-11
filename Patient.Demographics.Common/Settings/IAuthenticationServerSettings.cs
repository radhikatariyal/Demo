namespace Patient.Demographics.Common.Settings
{
    public interface IAuthenticationServerSettings
    {
        string Issuer { get; }
        string AudienceId { get; }
        string AudienceSecret { get; }
        int MaxFailedAttempts { get; }
        int PasswordExpiryDays { get; }
        double AuthenticationExpirationMinutes { get; }
        double RefreshTokenLifetimeMinutes { get; }
    }
}