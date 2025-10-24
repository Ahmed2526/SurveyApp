using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BussinessLogicLater.Service
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public RolesService(RoleManager<ApplicationRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<RoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

            var roleResponses = new List<RoleResponse>();

            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);

                var response = new RoleResponse
                {
                    RoleId = role.Id,
                    Name = role.Name!,
                    IsDeleted = role.IsDeleted,
                    Permissions = claims
                        .Select(c => c.Value)
                        .ToList()
                };

                roleResponses.Add(response);
            }

            return Result<IEnumerable<RoleResponse>>.Success(StatusCodes.Status200OK, roleResponses);
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(string RoleId, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role is null)
                return Result<RoleResponse>.Fail(StatusCodes.Status404NotFound, new[] { "Role not found" });

            var roleResponse = _mapper.Map<RoleResponse>(role);

            var rolePermissions = await _roleManager.GetClaimsAsync(role);

            roleResponse.Permissions = rolePermissions.Select(e => e.Value);

            return Result<RoleResponse>.Success(StatusCodes.Status200OK, roleResponse);
        }

        public async Task<Result<RoleResponse>> CreateAsync(RoleRequest request, CancellationToken cancellationToken)
        {
            var checkExist = await _roleManager.FindByNameAsync(request.Name);

            if (checkExist is not null)
                return Result<RoleResponse>.Fail(StatusCodes.Status409Conflict, new[] { "Role already exist" });

            //validate the submitted permissions
            List<string> validPermissions = ApplicationClaims.All().ToList();
            List<string> submittedPermissions = request.Permissions;

            var invalidPermissions = submittedPermissions
                .Where(p => !validPermissions.Contains(p))
                .ToList();

            if (invalidPermissions.Any())
            {
                return Result<RoleResponse>.Fail(StatusCodes.Status400BadRequest,
                    new[] { $"Invalid permissions: {string.Join(", ", invalidPermissions)}" });
            }


            //Create The Role
            var role = new ApplicationRole
            {
                Name = request.Name,
                NormalizedName = request.Name.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var CreateResult = await _roleManager.CreateAsync(role);

            if (!CreateResult.Succeeded)
            {
                var errors = CreateResult.Errors.Select(e => e.Description).ToArray();
                return Result<RoleResponse>.Fail(StatusCodes.Status409Conflict, errors);
            }

            //Add Permissions
            foreach (var permission in submittedPermissions)
            {
                var claimResult = await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaims.Type, permission));
                if (!claimResult.Succeeded)
                {
                    var claimErrors = claimResult.Errors.Select(e => e.Description).ToArray();
                    return Result<RoleResponse>.Fail(StatusCodes.Status400BadRequest, claimErrors);
                }
            }

            var roleResponse = _mapper.Map<RoleResponse>(role);

            roleResponse.Permissions = submittedPermissions;

            return Result<RoleResponse>.Success(StatusCodes.Status201Created, roleResponse);
        }

        public async Task<Result<bool>> UpdateAsync(string RoleId, RoleRequest request, CancellationToken cancellationToken)
        {
            // Check if role exists
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Role not found." });


            // Check if another role already has this name
            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole is not null && existingRole.Id != role.Id)
                return Result<bool>.Fail(StatusCodes.Status409Conflict, new[] { "Another role with this name already exists." });


            // Validate submitted permissions
            var validPermissions = ApplicationClaims.All().ToHashSet(StringComparer.OrdinalIgnoreCase);
            var submittedPermissions = request.Permissions ?? new List<string>();

            var invalidPermissions = submittedPermissions
                .Where(p => !validPermissions.Contains(p))
                .ToList();

            if (invalidPermissions.Any())
            {
                return Result<bool>.Fail(StatusCodes.Status400BadRequest,
                    new[] { $"Invalid permissions: {string.Join(", ", invalidPermissions)}" });
            }


            //update role name
            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpperInvariant();

            var updateResult = await _roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors.Select(e => e.Description).ToArray();
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, errors);
            }


            // Remove old claims
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in existingClaims)
                await _roleManager.RemoveClaimAsync(role, claim);


            // Add the new set of claims
            foreach (var permission in submittedPermissions)
            {
                var addClaimResult = await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaims.Type, permission));
                if (!addClaimResult.Succeeded)
                {
                    var claimErrors = addClaimResult.Errors.Select(e => e.Description).ToArray();
                    return Result<bool>.Fail(StatusCodes.Status400BadRequest, claimErrors);
                }
            }

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

        public async Task<Result<bool>> ToggleAsync(string RoleId, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Role not found" });

            role.IsDeleted = !role.IsDeleted;
            var UpdateResult = await _roleManager.UpdateAsync(role);

            if (!UpdateResult.Succeeded)
            {
                var errors = UpdateResult.Errors.Select(e => e.Description).ToArray();
                return Result<bool>.Fail(StatusCodes.Status409Conflict, errors);
            }

            return Result<bool>.Success(StatusCodes.Status202Accepted, true);
        }

    }
}
