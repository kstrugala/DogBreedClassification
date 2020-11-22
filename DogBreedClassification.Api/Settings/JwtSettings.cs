using Microsoft.Extensions.Configuration;

namespace DogBreedClassification.Api.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; }
        public double ExpiryMinutes { get; }
        public string Issuer { get; }
        public bool ValidateLifetime { get; }

        public JwtSettings(IConfiguration configuration)
        {
            SecretKey = configuration["Jwt:SecretKey"];
            ExpiryMinutes = configuration.GetSection("Jwt").GetValue<int>("ExpiryMinutes");
            Issuer = configuration["Jwt:Issuer"];
            ValidateLifetime  = configuration.GetSection("Jwt").GetValue<bool>("ValidateLifetime");
        }
    }
}
