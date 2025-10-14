using DataAccessLayer.DTOs;
using DataAccessLayer.Models;

namespace BussinessLogicLater.IService
{
    public interface IVotesService
    {
        Task<Result<bool>> Vote(int PollId, VoteRequest voteRequest, CancellationToken cancellationToken);
    }
}
