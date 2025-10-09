using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BussinessLogicLater.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> options, IRefreshTokenRepository refreshTokenRepository, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _jwtSettings = options.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<AuthResult>> RegisterAsync(RegisterDto registerCredentials, CancellationToken cancellationToken)
        {
            var CheckExist = await _userManager.FindByEmailAsync(registerCredentials.Email);

            if (CheckExist is not null)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Email already registered" });

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

                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, errors.ToArray());
            }

            var jwtToken = GenerateJWTtoken(user);
            var refreshToken = GenerateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            var authResult = new AuthResult()
            {
                JwtToken = jwtToken.Token,
                ExpiryAt = jwtToken.ExpiryAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiryAt = DateTime.UtcNow.AddDays(_jwtSettings.refreshTokenExpiryInDays)
            };

            return Result<AuthResult>.Success(StatusCodes.Status200OK, authResult);
        }

        public async Task<Result<AuthResult>> LoginAsync(LoginDto loginCredentials, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(loginCredentials.Email);

            if (user is null)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid email or password" });

            var isValidCredentials = await _userManager.CheckPasswordAsync(user, loginCredentials.Password);

            if (!isValidCredentials)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid email or password" });

            var jwtToken = GenerateJWTtoken(user);
            var refreshToken = GenerateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            var authResult = new AuthResult()
            {
                JwtToken = jwtToken.Token,
                ExpiryAt = jwtToken.ExpiryAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiryAt = DateTime.UtcNow.AddDays(_jwtSettings.refreshTokenExpiryInDays)
            };

            return Result<AuthResult>.Success(StatusCodes.Status200OK, authResult);

        }

        public async Task<Result<AuthResult>> RefreshToken(string refreshToken, string oldToken, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GettokenAsync(refreshToken, cancellationToken);

            if (token is null || !token.IsActive)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid token" });

            if (token.UserId != GetUserFromClaims(oldToken))
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid token" });

            var user = await _userManager.FindByIdAsync(token.UserId);

            if (user is null)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid user" });

            token.Revoked = DateTime.UtcNow;

            var Newtoken = GenerateJWTtoken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id);

            _refreshTokenRepository.Update(token);
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            var authResult = new AuthResult()
            {
                JwtToken = Newtoken.Token,
                ExpiryAt = Newtoken.ExpiryAt,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiryAt = DateTime.UtcNow.AddDays(_jwtSettings.refreshTokenExpiryInDays)
            };

            return Result<AuthResult>.Success(StatusCodes.Status200OK, authResult);
        }

        public async Task<Result<bool>> RevokeToken(string refreshToken, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GettokenAsync(refreshToken, cancellationToken);

            if (token is null || !token.IsActive)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid Token" });

            token.Revoked = DateTime.UtcNow;

            _refreshTokenRepository.Update(token);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

        //Log Error
        private string? GetUserFromClaims(string oldtoken)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false, //IMPORTANT: ignore expiration

                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(oldtoken, tokenValidationParameters, out SecurityToken securityToken);

                // ensure it's really a JWT token
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal.FindFirstValue(ClaimTypes.NameIdentifier);

            }
            catch (Exception)
            {

            }

            return null;
        }
        private JwtTokenResult GenerateJWTtoken(ApplicationUser user)
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
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.JwtExpiryInMinutes)
            );

            var tokenResult = new JwtTokenResult()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiryAt = DateTime.UtcNow.AddMinutes(_jwtSettings.JwtExpiryInMinutes)
            };

            return tokenResult;
        }
        private RefreshToken GenerateRefreshToken(string userId)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.refreshTokenExpiryInDays)
            };
        }


    }
}
