using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> options, IRefreshTokenRepository refreshTokenRepository, IHttpContextAccessor contextAccessor, ILogger<AuthService> logger, IEmailService emailService, IConfiguration config)
        {
            _userManager = userManager;
            _jwtSettings = options.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _emailService = emailService;
            _config = config;
        }

        public async Task<Result<bool>> RegisterAsync(RegisterDto registerCredentials, CancellationToken cancellationToken)
        {
            var CheckExist = await _userManager.FindByEmailAsync(registerCredentials.Email);

            if (CheckExist is not null)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "Email already registered" });

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

                return Result<bool>.Fail(StatusCodes.Status400BadRequest, errors.ToArray());
            }

            //Send Confirmation Email
            return await sendConfirmEmailAsync(user, cancellationToken);
        }
        public async Task<Result<AuthResult>> LoginAsync(LoginDto loginCredentials, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(loginCredentials.Email);

            if (user is null)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid email or password" });

            var isValidCredentials = await _userManager.CheckPasswordAsync(user, loginCredentials.Password);

            if (!isValidCredentials)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid email or password" });

            if (!user.EmailConfirmed)
                return Result<AuthResult>
                    .Fail(StatusCodes.Status403Forbidden, new[] { "Email not confirmed. Please confirm your email in order to signin" });

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

        public async Task<Result<AuthResult>> ConfirmEmail(string UserId, string Token, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user is null)
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid user/token" });

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                return Result<AuthResult>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid user/token" });
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
        public async Task<Result<bool>> ResendConfirmEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "User not found" });

            if (await _userManager.IsEmailConfirmedAsync(user))
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "Email already confirmed" });

            //Send Confirmation Email
            return await sendConfirmEmailAsync(user, cancellationToken);
        }

        public async Task<Result<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var email = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

            if (email is null)
                return Result<bool>.Fail(StatusCodes.Status401Unauthorized, new[] { "Unauthorized user" });

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid email or password" });

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Result<bool>.Fail(StatusCodes.Status403Forbidden, new[] { "Email must be confirmed before changing password" });

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var errorlist = new List<string>();

                foreach (var error in result.Errors)
                    errorlist.Add(error.Description);

                return Result<bool>.Fail(StatusCodes.Status400BadRequest, errorlist.ToArray());
            }

            _logger.LogInformation("User {Email} changed their password successfully.", user.Email);

            //revoke tokens after password change
            await _refreshTokenRepository.DeleteTokensForUserAsync(user.Id, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, result.Succeeded);
        }

        public async Task<Result<bool>> RequestResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "User not found" });

            //Send Reset Password Email
            return await sendResetPasswordEmailAsync(user, cancellationToken);
        }
        public async Task<Result<bool>> ResetPasswordAsync(ResetPassword request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid or expired reset request" });

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message, "Invalid token from user:{Email}", user.Email);

                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "Invalid token format" });
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!resetResult.Succeeded)
            {
                var errors = resetResult.Errors.Select(e => e.Description).ToArray();
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, errors);
            }

            //Invalidate all existing refresh tokens
            await _refreshTokenRepository.DeleteTokensForUserAsync(user.Id, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Email} successfully reset their password.", user.Email);

            //Get ip and deviceInfo
            var ip = _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var deviceInfo = _contextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

            await SendPasswordResetSuccessEmailAsync(user, ip, deviceInfo, cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
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


        //Handle frontend url
        private async Task<Result<bool>> sendConfirmEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontEndUrl = _config["FrontEnd:BaseUrl"];
            var confirmationUrl = $"{frontEndUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            // Get the absolute path to wwwroot
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Build the path to the template file
            var templatePath = Path.Combine(webRootPath, "EmailTemplates", "ConfirmEmail.html");

            // Read the HTML file
            var htmlBody = await File.ReadAllTextAsync(templatePath, cancellationToken);

            //Replace placeholders
            htmlBody = htmlBody
                .Replace("{{UserName}}", user.UserName ?? "User")
                .Replace("{{ConfirmationUrl}}", confirmationUrl);

            var emailResponse = await _emailService.SendEmailAsync(
                user.Email!,
                "Confirm your SurveyApp account",
                htmlBody,
                cancellationToken
            );

            if (!emailResponse.IsSuccess)
                return Result<bool>.Fail(emailResponse.StatusCode, emailResponse.Errors!);

            return Result<bool>.Success(emailResponse.StatusCode, emailResponse.Data);
        }
        private async Task<Result<bool>> sendResetPasswordEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontEndUrl = _config["FrontEnd:BaseUrl"];
            var confirmationUrl = $"{frontEndUrl}/reset-password?userId={user.Id}&token={encodedToken}";

            // Get the absolute path to wwwroot
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Build the path to the template file
            var templatePath = Path.Combine(webRootPath, "EmailTemplates", "ResetPassword.html");

            // Read the HTML file
            var htmlBody = await File.ReadAllTextAsync(templatePath, cancellationToken);

            //Replace placeholders
            htmlBody = htmlBody
                .Replace("{{UserName}}", user.UserName ?? "User")
                .Replace("{{ResetPasswordUrl}}", confirmationUrl);

            var emailResponse = await _emailService.SendEmailAsync(
                user.Email!,
                "Request-Reset your SurveyApp account password",
                htmlBody,
                cancellationToken
            );

            if (!emailResponse.IsSuccess)
                return Result<bool>.Fail(emailResponse.StatusCode, emailResponse.Errors!);

            return Result<bool>.Success(emailResponse.StatusCode, emailResponse.Data);
        }
        private async Task<Result<bool>> SendPasswordResetSuccessEmailAsync(ApplicationUser user, string ipAddress, string deviceInfo, CancellationToken cancellationToken)
        {
            var frontEndUrl = _config["FrontEnd:BaseUrl"];
            var loginUrl = $"{frontEndUrl}/login";

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var templatePath = Path.Combine(webRootPath, "EmailTemplates", "PasswordResetSuccess.html");

            var htmlBody = await File.ReadAllTextAsync(templatePath, cancellationToken);

            htmlBody = htmlBody
                .Replace("{{UserName}}", user.UserName)
                .Replace("{{UserEmail}}", user.Email)
                .Replace("{{LoginUrl}}", loginUrl)
                .Replace("{{ResetDateTime}}", DateTime.UtcNow.ToString("f") + " UTC")
                .Replace("{{IPAddress}}", ipAddress ?? "Unknown")
                .Replace("{{DeviceInfo}}", deviceInfo ?? "Unknown");

            // Step 5: Send email
            var emailResponse = await _emailService.SendEmailAsync(
                user.Email!,
                "Your SurveyApp Password Has Been Reset Successfully ✅",
                htmlBody,
                cancellationToken
            );

            if (!emailResponse.IsSuccess)
                return Result<bool>.Fail(emailResponse.StatusCode, emailResponse.Errors!);

            return Result<bool>.Success(emailResponse.StatusCode, emailResponse.Data);
        }


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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while decoding the token");
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
