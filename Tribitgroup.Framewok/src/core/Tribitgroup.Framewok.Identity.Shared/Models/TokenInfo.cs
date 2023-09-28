namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class TokenInfo
    {
        public string Token { get; set; } = string.Empty;
        public uint ExpiresInSeconds { get; set; } = 0;
        public string RefreshToken { get; set; } = string.Empty;
    }
}