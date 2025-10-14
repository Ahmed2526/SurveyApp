using DataAccessLayer.Data;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context) : base(context)
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

        public async Task<IEnumerable<Question>> GetAllWithIncludeAsync(int PollId, CancellationToken cancellationToken, params string[] includes)
        {
            var query = _context.Questions.Where(e => e.PollId == PollId).AsNoTracking();

            foreach (var include in includes)
                query = query.Include(include);

            var result = await query.ToListAsync(cancellationToken);

            return result;
        }

        public async Task<Question> GetByIdWithIncludeAsync(int PollId, int id, CancellationToken cancellationToken, params string[] includes)
        {
            IQueryable<Question> query = _context.Questions.Where(e => e.PollId == PollId).AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            var result = await query.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

            return result;
        }

        public async Task<bool> CheckUniqueQuestionInPollForCreate(int PollId, string Content, CancellationToken cancellationToken)
        {
            var exists = await _context.Questions
                 .AnyAsync(e => e.PollId == PollId && e.Content.ToLower() == Content.ToLower(), cancellationToken);

            return !exists;
        }

        public async Task<bool> CheckUniqueQuestionInPollForUpdate(int PollId, int QuestionId, string Content, CancellationToken cancellationToken)
        {
            var isDuplicateQuestion = await _context.Questions
                 .AnyAsync(e => e.PollId == PollId && e.Id != QuestionId &&
                 e.Content.Trim().ToLower() ==
                 Content.Trim().ToLower(), cancellationToken);

            return isDuplicateQuestion;
        }

        public bool CheckUniqueAnswers(IEnumerable<string> Answers)
        {
            if (Answers == null || !Answers.Any())
                return false;

            var normalized = Answers
                .Select(a => a.Trim().ToLower())
                .ToList();

            return !(normalized.Distinct().Count() == normalized.Count);
        }


    }
}
