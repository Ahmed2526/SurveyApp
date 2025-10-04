using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IAuthService
    {
        Task<Result<AuthResult>> RegisterAsync(RegisterDto registerCredentials);
        Task<Result<AuthResult>> LoginAsync(LoginDto loginCredentials);
        Task<Result<AuthResult>> RefreshToken(string Refreshtoken, string oldtoken);

        Task<Result<bool>> RevokeToken(string Refreshtoken);


    }
}
