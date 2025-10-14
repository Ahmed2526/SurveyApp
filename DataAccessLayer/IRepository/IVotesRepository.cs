using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IVotesRepository : IRepository<Poll>
    {
        Task<bool> IsPollActiveAsync(int pollId, CancellationToken cancellationToken);
        Task<bool> HasUserVotedAsync(int pollId, string userId, CancellationToken cancellationToken);
        Task<List<int>> GetPollQuestionIdsAsync(int pollId, CancellationToken cancellationToken);
        Task<int> GetQuestionCountAsync(int pollId, CancellationToken cancellationToken);
        Task<List<int>> GetSubmittedQuestionIdsAsync(VoteRequest voteRequest);
        Task AddVoteAsync(Vote vote, CancellationToken cancellationToken);
    }
}
