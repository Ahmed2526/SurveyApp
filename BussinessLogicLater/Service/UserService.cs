using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BussinessLogicLater.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<UserProfileResponse>> GetUserProfile(CancellationToken cancellationToken)
        {
            var UserId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId is null)
                return Result<UserProfileResponse>.Fail(StatusCodes.Status401Unauthorized, new[] { "Unauthorized user" });

            var UserInfo = await _userManager.Users.Where(e => e.Id == UserId)
                .Select(e => new UserProfileResponse()
                {
                    UserName = e.UserName!,
                    Email = e.Email!,
                    Phone = e.PhoneNumber!
                }).FirstOrDefaultAsync(cancellationToken);

            return Result<UserProfileResponse>.Success(StatusCodes.Status200OK, UserInfo!);
        }

        public async Task<Result<bool>> UpdateUserProfile(UserProfileRequest request, CancellationToken cancellationToken)
        {
            var UserId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId is null)
                return Result<bool>.Fail(StatusCodes.Status401Unauthorized, new[] { "Unauthorized user" });

            var rowsAffected = await _userManager.Users
                .Where(e => e.Id == UserId)
                .ExecuteUpdateAsync(e => e.SetProperty(m => m.PhoneNumber, request.Phone));

            if (rowsAffected < 1)
                return Result<bool>.Fail(StatusCodes.Status401Unauthorized, new[] { "Update failed. Please try again." });

            return Result<bool>.Success(StatusCodes.Status202Accepted, true);
        }
    }
}
