using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IAuthService
    {
        Task<Result<JwtAuthResult>> RegisterAsync(RegisterDto registerCredentials);
        Task<Result<JwtAuthResult>> LoginAsync(LoginDto loginCredentials);

    }
}
