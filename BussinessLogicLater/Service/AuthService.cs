using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BussinessLogicLater.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> options)
        {
            _userManager = userManager;
            _jwtSettings = options.Value;
        }

        public async Task<Result<JwtAuthResult>> RegisterAsync(RegisterDto registerCredentials)
        {
            var CheckExist = await _userManager.FindByEmailAsync(registerCredentials.Email);

            if (CheckExist is not null)
                return Result<JwtAuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Email already registered" });

            var user = new ApplicationUser()
            {
                UserName = registerCredentials.UserName,
                Email = registerCredentials.Email,
                PhoneNumber = registerCredentials.Phone
            };

            var result = await _userManager.CreateAsync(user, registerCredentials.Password);

            if (!result.Succeeded)
            {
                List<string> errors = new();

                foreach (var error in result.Errors)
                    errors.Add(error.Description);

                return Result<JwtAuthResult>.Fail(StatusCodes.Status400BadRequest, errors.ToArray());
            }

            var token = GenerateJWTtoken(user);
            return Result<JwtAuthResult>.Success(StatusCodes.Status200OK, token);
        }

        public async Task<Result<JwtAuthResult>> LoginAsync(LoginDto loginCredentials)
        {
            var user = await _userManager.FindByEmailAsync(loginCredentials.Email);

            if (user is null)
                return Result<JwtAuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid email or password" });

            var isValidCredentials = await _userManager.CheckPasswordAsync(user, loginCredentials.Password);

            if (!isValidCredentials)
                return Result<JwtAuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid email or password" });

            var token = GenerateJWTtoken(user);
            return Result<JwtAuthResult>.Success(StatusCodes.Status200OK, token);

        }

        public JwtAuthResult GenerateJWTtoken(ApplicationUser user)
        {

            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Role,"User")
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: SigningCredentials,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpiryDays)
            );

            var tokenResult = new JwtAuthResult()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiryInDays = DateTime.UtcNow.AddDays(_jwtSettings.ExpiryDays)
            };

            return tokenResult;
        }
    }
}
