using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class VotesRepository : Repository<Poll>, IVotesRepository
    {
        public VotesRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<bool> IsPollActiveAsync(int pollId, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.Polls.AnyAsync(p =>
                p.Id == pollId &&
                p.IsPublished &&
                p.StartsAt < today &&
                p.EndsAt > today, cancellationToken);
        }

        public async Task<bool> HasUserVotedAsync(int pollId, string userId, CancellationToken cancellationToken)
        {
            return await _context.Votes
                .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
        }

        public async Task<List<int>> GetPollQuestionIdsAsync(int pollId, CancellationToken cancellationToken)
        {
            return await _context.Questions
                .Where(q => q.PollId == pollId)
                .Select(q => q.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetQuestionCountAsync(int pollId, CancellationToken cancellationToken)
        {
            return await _context.Questions
                .CountAsync(q => q.PollId == pollId, cancellationToken);
        }

        public Task<List<int>> GetSubmittedQuestionIdsAsync(VoteRequest voteRequest)
        {
            var submittedQuestionIds = voteRequest.Votes
                .Select(v => v.QuestionId)
                .Distinct()
                .ToList();

            return Task.FromResult(submittedQuestionIds);
        }

        public async Task AddVoteAsync(Vote vote, CancellationToken cancellationToken)
        {
            await _context.AddAsync(vote, cancellationToken);
        }

    }
}
