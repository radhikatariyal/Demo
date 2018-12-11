namespace Patient.Demographics.Common.Settings
{
    public interface IWebsiteSettings
    {
        string ApiUrl { get; }
        string AuthenticationServer { get; }
        string SiteRoot { get; }
        bool ApiAutoWarmup { get; }
    }
}