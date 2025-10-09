using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IPollsService
    {
        Task<Result<IEnumerable<PollDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<PollDto>> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Result<PollDto>> CreateAsync(PollCreateDto dto, CancellationToken cancellationToken);
        Task<Result<bool>> UpdateAsync(int id, PollCreateDto dto, CancellationToken cancellationToken);
        Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
