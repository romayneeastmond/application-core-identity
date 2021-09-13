namespace ApplicationCoreIdentity.Services.Authentication
{
    public interface IAuthenticationConfiguration
    {
        string Issuer { get; }

        string Audience { get; }

        string Key { get; }
    }
}
