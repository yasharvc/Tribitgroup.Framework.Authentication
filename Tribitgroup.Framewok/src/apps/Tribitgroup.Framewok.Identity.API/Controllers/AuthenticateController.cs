using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tribitgroup.Framewok.Identity.Shared.DTO;
using Tribitgroup.Framewok.Identity.Shared.Exceptions;
using Tribitgroup.Framewok.Identity.Shared.Interfaces;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly JwtSetting _jwtSetting;
        private readonly StandardDbContext identityDbContext;
        private readonly IIdentityServerService _identityServerService;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ITokenGenerator tokenGenerator,
            JwtSetting jwtSetting,
            StandardDbContext identityDbContext,
            IIdentityServerService identityServerService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
            _jwtSetting = jwtSetting;
            this.identityDbContext = identityDbContext;
            _identityServerService = identityServerService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterWithUsernameEmailPasswordInputDTO model)
        {
            try
            {
                var userId = await _identityServerService.RegisterAsync(model);
                var roleId = await _identityServerService.AddRoleAsync("Admin");

                await _identityServerService.AddRoleToUserAsync(userId, "Admin");

                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (UserExistsException)
            {
                return BadRequest("User already exists");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterWithUsernameEmailPasswordInputDTO model)
        {
            
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("add-permission")]
        public async Task<IActionResult> AddPermissionAsync([FromBody] string newOne)
        {
            await _identityServerService.AddPermissionAsync<ApplicationPermission>(new ApplicationPermission(newOne));
            return Ok();
        }



            [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            string username = principal.Identity?.Name ?? "";

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var refreshTokenInDb = await identityDbContext.Set<UserRefreshToken>().FirstOrDefaultAsync(m=>m.RefreshToken == refreshToken && m.ValidUntil > DateTime.UtcNow);
            if(refreshTokenInDb is null)
                return BadRequest("Invalid access token or refresh token");

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            refreshTokenInDb.UsedCount++;
            identityDbContext.Update(refreshTokenInDb);

            await identityDbContext.Set<UserRefreshToken>().AddAsync(new UserRefreshToken
            {
                RefreshToken = newRefreshToken,
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                UsedCount = 0,
                ApplicationUserId = user.Id,
                ValidUntil = DateTime.UtcNow.AddMinutes(_jwtSetting.RefreshTokenExpiresInMinutes),
            });

            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }

        //[Authorize]
        //[HttpPost]
        //[Route("revoke/{username}")]
        //public async Task<IActionResult> Revoke(string username)
        //{
        //    var user = await _userManager.FindByNameAsync(username);
        //    if (user == null) return BadRequest("Invalid user name");

        //    user.RefreshToken = null;
        //    await _userManager.UpdateAsync(user);

        //    return NoContent();
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("revoke-all")]
        //public async Task<IActionResult> RevokeAll()
        //{
        //    var users = _userManager.Users.ToList();
        //    foreach (var user in users)
        //    {
        //        user.RefreshToken = null;
        //        await _userManager.UpdateAsync(user);
        //    }

        //    return NoContent();
        //}

        [Authorize(Roles ="Role1")]
        [HttpGet]
        [Route("reserved-data")]
        public IActionResult GetData() => new JsonResult(new { result = true });


        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret));
            uint tokenValidityInSeconds = _jwtSetting.ExpiresInSeconds;

            var token = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                expires: DateTime.Now.AddSeconds(tokenValidityInSeconds),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

    }
    public class TokenModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}