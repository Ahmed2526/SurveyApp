using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IPollsService
    {
        Task<Result<IEnumerable<PollDto>>> GetAllAsync();
        Task<Result<PollDto>> GetByIdAsync(int id);
        Task<Result<PollDto>> CreateAsync(PollCreateDto dto);
        Task<Result<bool>> UpdateAsync(int id, PollCreateDto dto);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
