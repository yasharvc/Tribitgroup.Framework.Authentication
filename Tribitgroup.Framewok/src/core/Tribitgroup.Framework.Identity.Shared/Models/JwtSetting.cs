using Tribitgroup.Framework.Shared.Entities;

namespace Tribitgroup.Framework.Identity.Shared.Models
{
    public sealed class JwtSetting : Entity
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public IEnumerable<string> Audiences { get; set; } = Enumerable.Empty<string>();
        public uint ExpiresInSeconds { get; set; }
        public uint RefreshTokenExpiresInMinutes { get; set; }
        public string Secret { get; set; } = string.Empty;
        public bool ValidateIssuer{get;set;}=true;
        public bool ValidateAudience{get;set;}=true;
        public bool ValidateLifetime{get;set;}=true;
        public bool ValidateIssuerSigningKey { get; set; } = true;
    }
}