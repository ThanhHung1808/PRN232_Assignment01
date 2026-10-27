using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FUNewsManagementAPI.DTOs;
using FUNewsManagementAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FUNewsManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISystemAccountRepository _accountRepository;
        private readonly AdminAccountConfig _adminConfig;
        private readonly IConfiguration _configuration;

        public AuthController(
            ISystemAccountRepository accountRepository,
            IOptions<AdminAccountConfig> adminConfig,
            IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _adminConfig = adminConfig.Value;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Check Admin Credentials from appsettings.json
            if (string.Equals(request.Email.Trim(), _adminConfig.Email.Trim(), StringComparison.OrdinalIgnoreCase)
                && request.Password == _adminConfig.Password)
            {
                var token = GenerateJwtToken("0", "Administrator", _adminConfig.Email, "Admin");
                return Ok(new LoginResponse
                {
                    AccountId = 0,
                    AccountName = "Administrator",
                    AccountEmail = _adminConfig.Email,
                    Role = "Admin",
                    AccountRole = null,
                    Token = token
                });
            }

            // 2. Check Database Accounts
            var account = await _accountRepository.LoginAsync(request.Email, request.Password);
            if (account == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            string roleName = account.AccountRole switch
            {
                1 => "Staff",
                2 => "Lecturer",
                _ => "User"
            };

            var userToken = GenerateJwtToken(
                account.AccountId.ToString(),
                account.AccountName ?? "User",
                account.AccountEmail ?? "",
                roleName
            );

            return Ok(new LoginResponse
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName ?? "",
                AccountEmail = account.AccountEmail ?? "",
                Role = roleName,
                AccountRole = account.AccountRole,
                Token = userToken
            });
        }

        private string GenerateJwtToken(string id, string name, string email, string role)
        {
            var key = _configuration["Jwt:Key"] ?? "PRN232_FUNewsManagement_SecretKey_For_Assignment01_2026_FPT!";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var m) ? m : 180;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "FUNewsManagementAPI",
                audience: _configuration["Jwt:Audience"] ?? "FUNewsManagementClient",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
