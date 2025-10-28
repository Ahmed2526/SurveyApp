using Azure.Core;
using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogicLater.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        //I KNOW THAT THIS WILL CAUSE N+1 PROBLEM!
        public async Task<Result<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);

            var responses = _mapper.Map<List<UserResponse>>(users);

            foreach (var response in responses)
                response.Roles = await _userManager.GetRolesAsync(users.Find(e => e.Id == response.UserId)!);

            return Result<IEnumerable<UserResponse>>.Success(StatusCodes.Status200OK, responses);
        }

        public async Task<Result<UserResponse>> GetByIdAsync(string UserId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user is null)
                return Result<UserResponse>.Fail(StatusCodes.Status404NotFound, new[] { "User not found" });

            var response = _mapper.Map<UserResponse>(user);

            response.Roles = await _userManager.GetRolesAsync(user);

            return Result<UserResponse>.Success(StatusCodes.Status200OK, response);

        }

        //handle send email to set password instead of manual setting
        public async Task<Result<bool>> CreateAsync(UserRequest request, CancellationToken cancellationToken)
        {
            var CheckExist = await _userManager.FindByEmailAsync(request.Email);

            if (CheckExist is not null)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "Email already registered" });

            //Validate submitted roles
            var validRoles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync(cancellationToken);

            var submittedRoles = request.Roles;

            var invalidRoles = submittedRoles
                .Where(r => !validRoles.Contains(r))
                .ToList();

            if (invalidRoles.Any())
            {
                return Result<bool>.Fail(StatusCodes.Status400BadRequest,
                    new[] { $"Invalid roles: {string.Join(", ", invalidRoles)}" });
            }

            //Create the user
            var user = new ApplicationUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                LockoutEnabled = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, errors);
            }

            var addtoRoleResult = await _userManager.AddToRolesAsync(user, request.Roles);

            if (!addtoRoleResult.Succeeded)
            {
                var errors = addtoRoleResult.Errors.Select(e => e.Description).ToArray();
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, errors);
            }

            return Result<bool>.Success(StatusCodes.Status201Created, true);
        }

        public async Task<Result<bool>> ToggleLockoutAsync(string UserId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "User not found" });

            // If user currently locked, unlock them
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow); // unlock immediately
                return Result<bool>.Success(StatusCodes.Status200OK, true);
            }
            else
            {
                // Otherwise, lock them permanently (e.g., 150 years)
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(150));
                return Result<bool>.Success(StatusCodes.Status200OK, true);
            }
        }

        public async Task<Result<bool>> EditUserRolesAsync(string UserId, List<string> Roles, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "User not found" });

            //Validate submitted roles
            var validRoles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync(cancellationToken);

            var submittedRoles = Roles;

            var invalidRoles = submittedRoles
                .Where(r => !validRoles.Contains(r))
                .ToList();

            if (invalidRoles.Any())
            {
                return Result<bool>.Fail(StatusCodes.Status400BadRequest,
                    new[] { $"Invalid roles: {string.Join(", ", invalidRoles)}" });
            }

            //Get current user roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            //Determine roles to remove and add
            var rolesToRemove = currentRoles.Except(Roles).ToList();
            var rolesToAdd = Roles.Except(currentRoles).ToList();

            //Remove old roles
            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    var errors = removeResult.Errors.Select(e => e.Description).ToArray();
                    return Result<bool>.Fail(StatusCodes.Status400BadRequest, errors);
                }
            }

            //Add new roles
            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    var errors = addResult.Errors.Select(e => e.Description).ToArray();
                    return Result<bool>.Fail(StatusCodes.Status400BadRequest, errors);
                }
            }

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }
    }
}
