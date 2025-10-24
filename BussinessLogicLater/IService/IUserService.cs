using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IUserService
    {
        Task<Result<UserProfileResponse>> GetUserProfile(CancellationToken cancellationToken);
        Task<Result<bool>> UpdateUserProfile(UserProfileRequest request, CancellationToken cancellationToken);
    }
}
