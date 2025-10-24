using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IAuthService
    {
        Task<Result<bool>> RegisterAsync(RegisterDto registerCredentials, CancellationToken cancellationToken);
        Task<Result<AuthResult>> LoginAsync(LoginDto loginCredentials, CancellationToken cancellationToken);

        Task<Result<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> RequestResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> ResetPasswordAsync(ResetPassword request, CancellationToken cancellationToken);

        Task<Result<AuthResult>> ConfirmEmail(string UserId, string Token, CancellationToken cancellationToken);
        Task<Result<bool>> ResendConfirmEmail(string email, CancellationToken cancellationToken);

        Task<Result<AuthResult>> RefreshToken(string Refreshtoken, string oldtoken, CancellationToken cancellationToken);
        Task<Result<bool>> RevokeToken(string Refreshtoken, CancellationToken cancellationToken);


    }
}
