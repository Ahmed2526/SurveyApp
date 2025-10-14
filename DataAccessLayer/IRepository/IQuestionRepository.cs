using DataAccessLayer.Models;

namespace DataAccessLayer.IRepository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<bool> IsPollActiveAsync(int pollId, CancellationToken cancellationToken);
        Task<bool> HasUserVotedAsync(int pollId, string userId, CancellationToken cancellationToken);
        Task<IEnumerable<Question>> GetAllWithIncludeAsync(int PollId, CancellationToken cancellationToken, params string[] includes);
        Task<Question> GetByIdWithIncludeAsync(int PollId, int id, CancellationToken cancellationToken, params string[] includes);
        Task<bool> CheckUniqueQuestionInPollForCreate(int PollId, string Content, CancellationToken cancellationToken);
        Task<bool> CheckUniqueQuestionInPollForUpdate(int PollId, int QuestionId, string Content, CancellationToken cancellationToken);
        bool CheckUniqueAnswers(IEnumerable<string> Answers);



    }
}
