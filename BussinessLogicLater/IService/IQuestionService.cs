using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IQuestionService
    {
        Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken);
        Task<Result<QuestionResponse>> GetByIdAsync(int PollId, int id, CancellationToken cancellationToken);
        Task<Result<QuestionResponse>> CreateAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> UpdateAsync(int PollId, int QuestionId, QuestionRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> DeleteAsync(int PollId, int QuestionId, CancellationToken cancellationToken);
    }
}
