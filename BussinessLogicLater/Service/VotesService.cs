using BussinessLogicLater.IService;
using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BussinessLogicLater.Service
{
    public class VotesService : IVotesService
    {
        private readonly IVotesRepository _votesRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public VotesService(IMapper mapper, IHttpContextAccessor contextAccessor, IVotesRepository votesRepository)
        {
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _votesRepository = votesRepository;
        }

        public async Task<Result<bool>> Vote(int PollId, VoteRequest voteRequest, CancellationToken cancellationToken)
        {

            var IsPollExist = await _votesRepository.IsPollActiveAsync(PollId, cancellationToken);

            if (!IsPollExist)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            //Validate User
            var userId = _contextAccessor.HttpContext!
               .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Result<bool>.Fail(StatusCodes.Status401Unauthorized, new[] { "unauthorized user" });


            var hasUserVotedBefore = await _votesRepository.HasUserVotedAsync(PollId, userId, cancellationToken);

            if (hasUserVotedBefore)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "User has already voted" });


            // Get number of questions in this poll
            int totalQuestionsInPoll = await _votesRepository.GetQuestionCountAsync(PollId, cancellationToken);

            int submittedQuestionsCount = voteRequest.Votes
                .Select(v => v.QuestionId)
                .Distinct()
                .Count();

            if (submittedQuestionsCount != totalQuestionsInPoll)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest, new[] { "You must answer all questions in this poll." });


            // Get all question IDs for this poll from the database
            var pollQuestionIds = await _votesRepository.GetPollQuestionIdsAsync(PollId, cancellationToken);

            // Get all submitted QuestionIds
            var submittedQuestionIds = await _votesRepository.GetSubmittedQuestionIdsAsync(voteRequest);

            // Check if both sets contain exactly the same QuestionIds
            bool sameQuestions = pollQuestionIds.Count == submittedQuestionIds.Count &&
                                 !pollQuestionIds.Except(submittedQuestionIds).Any() &&
                                 !submittedQuestionIds.Except(pollQuestionIds).Any();

            if (!sameQuestions)
                return Result<bool>.Fail(StatusCodes.Status400BadRequest,
                    new[] { "Submitted questions do not match the poll questions." });


            var vote = new Vote()
            {
                PollId = PollId,
                UserId = userId
            };

            List<VoteAnswer> voteAnswers = new();

            foreach (var voterequest in voteRequest.Votes)
            {
                voteAnswers.Add(new VoteAnswer()
                {
                    QuestionId = voterequest.QuestionId,
                    AnswerId = voterequest.AnswerId
                });
            }

            vote.Answers = voteAnswers;

            await _votesRepository.AddVoteAsync(vote, cancellationToken);
            await _votesRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(StatusCodes.Status201Created, true);
        }


    }
}
