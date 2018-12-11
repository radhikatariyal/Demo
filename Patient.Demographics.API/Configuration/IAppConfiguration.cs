namespace Patient.Demographics.API.Configuration
{
    public interface IAppConfiguration
    {
        string AcceptableDomains { get; }
        bool AutoMigrateDatabase { get; }
        string ConnectionString { get; }
    }
}