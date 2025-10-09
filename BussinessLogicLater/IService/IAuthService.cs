using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IAuthService
    {
        Task<Result<AuthResult>> RegisterAsync(RegisterDto registerCredentials, CancellationToken cancellationToken);
        Task<Result<AuthResult>> LoginAsync(LoginDto loginCredentials, CancellationToken cancellationToken);
        Task<Result<AuthResult>> RefreshToken(string Refreshtoken, string oldtoken, CancellationToken cancellationToken);

        Task<Result<bool>> RevokeToken(string Refreshtoken, CancellationToken cancellationToken);


    }
}
