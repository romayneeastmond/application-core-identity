namespace ApplicationCoreIdentity.Services.Authentication
{
    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }
    }
}
