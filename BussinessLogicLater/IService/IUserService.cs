using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<UserResponse>> GetByIdAsync(string UserId, CancellationToken cancellationToken);
        Task<Result<bool>> CreateAsync(UserRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> ToggleLockoutAsync(string UserId, CancellationToken cancellationToken);
        Task<Result<bool>> EditUserRolesAsync(string UserId, List<string> Roles, CancellationToken cancellationToken);

    }
}
