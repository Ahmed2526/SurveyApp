using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IRolesService
    {
        Task<Result<IEnumerable<RoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken);
        Task<Result<RoleResponse>> GetByIdAsync(string RoleId, CancellationToken cancellationToken);
        Task<Result<RoleResponse>> CreateAsync(RoleRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> UpdateAsync(string RoleId, RoleRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> ToggleAsync(string RoleId, CancellationToken cancellationToken);
    }
}
