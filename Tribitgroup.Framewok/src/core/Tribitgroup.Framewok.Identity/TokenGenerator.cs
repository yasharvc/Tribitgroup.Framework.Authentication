using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tribitgroup.Framewok.Identity.Interfaces;
using Tribitgroup.Framewok.Identity.Models;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public class TokenGenerator: ITokenGenerator
    {
        JwtSetting JwtSetting { get; }
        SymmetricSecurityKey AuthSigningKey => new(Encoding.UTF8.GetBytes(JwtSetting.Secret));
        public TokenGenerator(JwtSetting jwtSetting)
        {
            JwtSetting = jwtSetting;
        }


        public Task<TokenInfo> GetTokenAsync(UserInfo userInfo)
        {
            var token = new JwtSecurityToken(
                issuer: JwtSetting.Issuer,
                audience: JwtSetting.Audience,
                expires: DateTime.Now.AddSeconds(JwtSetting.ExpiresInSeconds),
                claims: GetClaims(userInfo),
                signingCredentials: new SigningCredentials(AuthSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var res = new TokenInfo
            {
                ExpiresInSeconds = JwtSetting.ExpiresInSeconds,
                RefreshToken = GenerateRefreshToken(),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return Task.FromResult(res);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static IEnumerable<Claim> GetClaims(UserInfo userInfo)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userInfo.Roles)
                authClaims.Add(new Claim(ClaimTypes.Role, userRole.Name ?? ""));

            foreach (var userTenant in userInfo.Tenants) {
                authClaims.Add(new Claim("tenant-name", userTenant.Title));
                authClaims.Add(new Claim("tenant-key", userTenant.ShortKey));
            }

            return authClaims;
        }
    }
}
